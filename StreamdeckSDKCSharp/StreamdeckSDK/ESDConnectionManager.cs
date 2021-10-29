using System;
using System.Buffers;
using System.Net.WebSockets;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Elgato.StreamdeckSDK.Types.Common;
using Elgato.StreamdeckSDK.Types.Events;
using Elgato.StreamdeckSDK.Types.Events.ESDActions;
using Elgato.StreamdeckSDK.Types.Messages;
using Elgato.StreamdeckSDK.Types.Messages.ContextualMessages;
using Elgato.StreamdeckSDK.Types.Payloads.Messages;

namespace Elgato.StreamdeckSDK
{
    public class ESDConnectionManager
    {
        private const int BUFFER_SIZE = 1024 * 1024;

        private int Port { get; }
        private string PluginUUID { get; }
        private string RegisterEvent { get; }

        private Task EventLoopTask { get; set; }
        private CancellationTokenSource CancellationTokenSource { get; set; }

        public bool DieOnException { get; set; }

        private ClientWebSocket WebSocket { get; set; }
        public ESDConnectionManager(int port, string pluginUUID, string registerEvent)
        {
            Port = port;
            PluginUUID = pluginUUID;
            RegisterEvent = registerEvent;
        }

        public event EventHandler<ESDKeyActionEventNotification> KeyUpForAction;
        public event EventHandler<ESDKeyActionEventNotification> KeyDownForAction;
        public event EventHandler<ESDAppearanceActionEventNotification> WillAppearForAction;
        public event EventHandler<ESDAppearanceActionEventNotification> WillDisappearForAction;
        public event EventHandler<ESDDeviceConnectEventNotification> DeviceDidConnect;
        public event EventHandler<ESDDeviceDisconnectEventNotification> DeviceDidDisconnect;
        public event EventHandler<ESDApplicationEventNotification> ApplicationDidLaunch;
        public event EventHandler<ESDApplicationEventNotification> ApplicationDidTerminate;
        public event EventHandler<ESDTitleParametersChangeActionEventNotification> TitleParametersChanged;
        public event EventHandler<ESDSendToPluginActionEventNotification> SendToPlugin;
        public event EventHandler<ESDSettingsActionEventNotification> DidReceiveSettings;
        public event EventHandler<ESDGlobalSettingsEventNotification> DidReceiveGlobalSettings;
        public event EventHandler<ESDPropertyInspectorAppearanceActionEventNotification> PropertyInspectorAppeared;
        public event EventHandler<ESDPropertyInspectorAppearanceActionEventNotification> PropertyInspectorDisappeared;

        public event EventHandler<Exception> ExceptionOccurredWhileReceiving; 

        public async Task Run()
        {
            if (EventLoopTask is { IsCompleted: false })
            {
                CancellationTokenSource.Cancel();
                await WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure,
                    $"Websocket was already running: ${PluginUUID}",
                    CancellationTokenSource.Token);
            }

            CancellationTokenSource = new CancellationTokenSource();

            WebSocket = new ClientWebSocket();
            Uri uri = new Uri("ws://127.0.0.1:" + Port);

            await WebSocket.ConnectAsync(uri, CancellationTokenSource.Token);

            ESDConnectMessage msg = new ESDConnectMessage { Event = RegisterEvent, UUID = PluginUUID };
            byte[] buffer = Serialize(msg);
            await WebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationTokenSource.Token);

            EventLoopTask = Task.Run(RunEventLoop, CancellationTokenSource.Token);

            await EventLoopTask;
        }

        private async Task RunEventLoop()
        {
            while (!CancellationTokenSource.IsCancellationRequested)
            {
                try
                {
                    await EventLoop();
                }
                catch (OperationCanceledException e)
                {
                    ErrorHandling(e);
                    break;
                }
                catch (Exception e)
                {
                    ErrorHandling(e);

                    if (DieOnException)
                        throw;
                }       
            }
        }

        private void ErrorHandling(Exception exception)
        {
            LogMessage($"Exception {exception.GetType()} occurred. Message: {exception.Message}").ConfigureAwait(false);

            ExceptionOccurredWhileReceiving?.Invoke(this, exception);
        }

        private async Task EventLoop()
        {
            using IMemoryOwner<byte> memoryOwner = MemoryPool<byte>.Shared.Rent(BUFFER_SIZE);
            Memory<byte> memory = memoryOwner.Memory;

            ValueWebSocketReceiveResult result = await WebSocket.ReceiveAsync(memory, CancellationToken.None);

            switch (result.MessageType)
            {
                case WebSocketMessageType.Binary:
                    throw new NotSupportedException($"MessageType {nameof(WebSocketMessageType.Binary)}");
                case WebSocketMessageType.Close:
                    CancellationTokenSource.Cancel();
                    CancellationTokenSource.Token.ThrowIfCancellationRequested();
                    break;
                case WebSocketMessageType.Text:
                    OnMessageTypeText(memory[..result.Count]);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnMessageTypeText(Memory<byte> memory)
        {
            ESDEventNotification notification = ESDEventNotification.Parse(memory);

            switch (notification.Event)
            {
                case ESDEvent.KeyUp: KeyUpForAction?.Invoke(this, notification as ESDKeyActionEventNotification); break;
                case ESDEvent.KeyDown: KeyDownForAction?.Invoke(this, notification as ESDKeyActionEventNotification); break;
                case ESDEvent.WillAppear: WillAppearForAction?.Invoke(this, notification as ESDAppearanceActionEventNotification); break;
                case ESDEvent.WillDisappear: WillDisappearForAction?.Invoke(this, notification as ESDAppearanceActionEventNotification); break;
                case ESDEvent.DeviceDidConnect: DeviceDidConnect?.Invoke(this, notification as ESDDeviceConnectEventNotification); break;
                case ESDEvent.DeviceDidDisconnect: DeviceDidDisconnect?.Invoke(this, notification as ESDDeviceDisconnectEventNotification); break;
                case ESDEvent.ApplicationDidLaunch: ApplicationDidLaunch?.Invoke(this, notification as ESDApplicationEventNotification); break;
                case ESDEvent.ApplicationDidTerminate: ApplicationDidTerminate?.Invoke(this, notification as ESDApplicationEventNotification); break;
                case ESDEvent.TitleParametersDidChange: TitleParametersChanged?.Invoke(this, notification as ESDTitleParametersChangeActionEventNotification); break;
                case ESDEvent.SendToPlugin: SendToPlugin?.Invoke(this, notification as ESDSendToPluginActionEventNotification); break;
                case ESDEvent.DidReceiveSettings: DidReceiveSettings?.Invoke(this, notification as ESDSettingsActionEventNotification); break;
                case ESDEvent.DidReceiveGlobalSettings: DidReceiveGlobalSettings?.Invoke(this, notification as ESDGlobalSettingsEventNotification); break;
                case ESDEvent.PropertyInspectorDidAppear: PropertyInspectorAppeared?.Invoke(this, notification as ESDPropertyInspectorAppearanceActionEventNotification); break;
                case ESDEvent.PropertyInspectorDidDisappear: PropertyInspectorDisappeared?.Invoke(this, notification as ESDPropertyInspectorAppearanceActionEventNotification); break;
                default: throw new ArgumentOutOfRangeException();
            }
        }

        public async Task SetTitle(string title, string context, ESDSDKTarget target)
        {
            ESDTitleMessage msg = new ESDTitleMessage { Context = context, Event = ESDFunction.SetTitle, Payload = new ESDTitleMessagePayload { Target = target, Title = title } };
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter<ESDFunction>(JsonNamingPolicy.CamelCase) }
            };
            byte[] buffer = Serialize(msg, options);

            await WebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationTokenSource.Token);
        }

        public async Task SetImage(string base64ImageString, string context, ESDSDKTarget target)
        {
            if (string.IsNullOrWhiteSpace(base64ImageString))
            {
                throw new ArgumentException(nameof(base64ImageString));
            }
            const string imagePrefix = "data:image/png;base64,";

            if (!base64ImageString.StartsWith(imagePrefix))
            {
                base64ImageString = $"{imagePrefix}{base64ImageString}";
            }

            ESDImageMessage msg = new ESDImageMessage { Context = context, Event = ESDFunction.SetImage, Payload = new ESDImageMessagePayload { Target = target, Image = base64ImageString } };
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter<ESDFunction>(JsonNamingPolicy.CamelCase) }
            };
            byte[] buffer = Serialize(msg, options);

            await WebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationTokenSource.Token);

        }

        public async Task ShowAlertForContext(string context)
        {
            ESDAlertMessage msg = new ESDAlertMessage { Context = context, Event = ESDFunction.ShowAlert };

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter<ESDFunction>(JsonNamingPolicy.CamelCase) }
            };
            byte[] buffer = Serialize(msg, options);
            await WebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationTokenSource.Token);
        }

        public async Task ShowOkForContext(string context)
        {
            ESDOkMessage msg = new ESDOkMessage { Context = context, Event = ESDFunction.ShowOk };

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter<ESDFunction>(JsonNamingPolicy.CamelCase) }
            };
            byte[] buffer = Serialize(msg, options);
            await WebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationTokenSource.Token);
        }

        public async Task SetSettings(JsonElement settings, string context)
        {
            ESDSettingsMessage msg = new ESDSettingsMessage { Context = context, Event = ESDFunction.SetSettings, Payload = settings };

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter<ESDFunction>(JsonNamingPolicy.CamelCase) }
            };
            byte[] buffer = Serialize(msg, options);
            await WebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationTokenSource.Token);
        }

        public async Task SetState(int state, string context)
        {
            ESDStateMessage msg = new ESDStateMessage { Context = context, Event = ESDFunction.SetState, Payload = new ESDStateMessagePayload { State = state } };

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter<ESDFunction>(JsonNamingPolicy.CamelCase) }
            };
            byte[] buffer = Serialize(msg, options);
            await WebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationTokenSource.Token);
        }

        public async Task SendToPropertyInspector(string action, string context, JsonElement payload)
        {
            ESDPropertyInspectorMessage msg = new ESDPropertyInspectorMessage { Context = context, Action = action, Event = ESDFunction.SendToPropertyInspector, Payload = payload };

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter<ESDFunction>(JsonNamingPolicy.CamelCase) }
            };
            byte[] buffer = Serialize(msg, options);
            await WebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationTokenSource.Token);

        }

        public async Task SwitchToProfile(string deviceId, string profileName)
        {
            if (string.IsNullOrWhiteSpace(deviceId))
                throw new ArgumentException(nameof(deviceId));

            if (string.IsNullOrWhiteSpace(profileName))
                throw new ArgumentException(nameof(profileName));

            ESDSwitchProfileMessage msg = new ESDSwitchProfileMessage { Context = PluginUUID, Device = deviceId, Event = ESDFunction.SwitchToProfile, Payload = new ESDProfileMessagePayload { Profile = profileName } };

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter<ESDFunction>(JsonNamingPolicy.CamelCase) }
            };
            byte[] buffer = Serialize(msg, options);
            await WebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationTokenSource.Token);
        }

        public async Task LogMessage(string message)
        {
            ESDLogMessage msg = new ESDLogMessage { Event = ESDFunction.LogMessage, Payload = new ESDLogMessagePayload { Message = message } };
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter<ESDFunction>(JsonNamingPolicy.CamelCase) }
            };
            byte[] buffer = Serialize(msg, options);
            await WebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationTokenSource.Token);
        }

        private static byte[] Serialize<T>(T toSerialize) => Serialize(toSerialize, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        private static byte[] Serialize<T>(T toSerialize, JsonSerializerOptions options) => JsonSerializer.SerializeToUtf8Bytes(toSerialize, options);

    }
}