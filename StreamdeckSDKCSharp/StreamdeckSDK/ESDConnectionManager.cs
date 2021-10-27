using System;
using System.Buffers;
using System.Net.WebSockets;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Elgato.StreamdeckSDK.Types.Common;
using Elgato.StreamdeckSDK.Types.Exceptions;
using Elgato.StreamdeckSDK.Types.Messages;
using Elgato.StreamdeckSDK.Types.Messages.ESDActions;

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

        private ClientWebSocket WebSocket { get; set; }
        public ESDConnectionManager(int port, string pluginUUID, string registerEvent)
        {
            Port = port;
            PluginUUID = pluginUUID;
            RegisterEvent = registerEvent;
        }

        public event EventHandler<ESDKeyActionMessage> KeyUpForAction;
        public event EventHandler<ESDKeyActionMessage> KeyDownForAction;
        public event EventHandler<ESDAppearanceActionMessage> WillAppearForAction;
        public event EventHandler<ESDAppearanceActionMessage> WillDisappearForAction;
        public event EventHandler<ESDDeviceConnectMessage> DeviceDidConnect;
        public event EventHandler<ESDDeviceDisconnectMessage> DeviceDidDisconnect;
        public event EventHandler<ESDApplicationMessage> ApplicationDidLaunch;
        public event EventHandler<ESDApplicationMessage> ApplicationDidTerminate;
        public event EventHandler<ESDTitleParametersChangeActionMessage> TitleParametersChanged;
        public event EventHandler<ESDSendToPluginActionMessage> SendToPlugin;
        public event EventHandler<ESDSettingsActionMessage> DidReceiveSettings;
        public event EventHandler<ESDGlobalSettingsMessage> DidReceiveGlobalSettings;
        public event EventHandler<ESDPropertyInspectorAppearanceActionMessage> PropertyInspectorAppeared;
        public event EventHandler<ESDPropertyInspectorAppearanceActionMessage> PropertyInspectorDisappeared;

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

            ConnectMessage msg = new ConnectMessage { Event = RegisterEvent, UUID = PluginUUID };
            byte[] buffer = Serialize(msg);
            await WebSocket.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationTokenSource.Token);

            EventLoopTask = Task.Run(RunEventLoop, CancellationTokenSource.Token);

            await EventLoopTask;
        }

        private async Task RunEventLoop()
        {
            while (!CancellationTokenSource.IsCancellationRequested)
            {
                await EventLoop();
            }
        }

        private async Task EventLoop()
        {
            using IMemoryOwner<byte> memoryOwner = MemoryPool<byte>.Shared.Rent(BUFFER_SIZE);
            Memory<byte> memory = memoryOwner.Memory;
            try
            {
                var result = await WebSocket.ReceiveAsync(memory, CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Text)
                {
                    OnMessageTypeText(memory[..result.Count]);
                }
                else if (result.MessageType == WebSocketMessageType.Close)
                {
                    //TODO close message type
                    throw new NotImplementedException();
                }
                else
                {
                    //TODO: Binary Message Type
                    throw new NotImplementedException();
                }
            }
            catch (JsonException)
            {
                //TODO: Log Error
            }
            catch (ESDSDKException)
            {
                //TODO: Log Error
            }
        }

        private void OnMessageTypeText(Memory<byte> memory)
        {
            ESDMessage message = ESDMessage.Parse(memory);

            switch (message.Event)
            {
                case ESDEvent.KeyUp: KeyUpForAction?.Invoke(this, message as ESDKeyActionMessage); break;
                case ESDEvent.KeyDown: KeyDownForAction?.Invoke(this, message as ESDKeyActionMessage); break;
                case ESDEvent.WillAppear: WillAppearForAction?.Invoke(this, message as ESDAppearanceActionMessage); break;
                case ESDEvent.WillDisappear: WillDisappearForAction?.Invoke(this, message as ESDAppearanceActionMessage); break;
                case ESDEvent.DeviceDidConnect: DeviceDidConnect?.Invoke(this, message as ESDDeviceConnectMessage); break;
                case ESDEvent.DeviceDidDisconnect: DeviceDidDisconnect?.Invoke(this, message as ESDDeviceDisconnectMessage); break;
                case ESDEvent.ApplicationDidLaunch: ApplicationDidLaunch?.Invoke(this, message as ESDApplicationMessage); break;
                case ESDEvent.ApplicationDidTerminate: ApplicationDidTerminate?.Invoke(this, message as ESDApplicationMessage); break;
                case ESDEvent.TitleParametersDidChange: TitleParametersChanged?.Invoke(this, message as ESDTitleParametersChangeActionMessage); break;
                case ESDEvent.SendToPlugin: SendToPlugin?.Invoke(this, message as ESDSendToPluginActionMessage); break;
                case ESDEvent.DidReceiveSettings: DidReceiveSettings?.Invoke(this, message as ESDSettingsActionMessage); break;
                case ESDEvent.DidReceiveGlobalSettings: DidReceiveGlobalSettings?.Invoke(this, message as ESDGlobalSettingsMessage); break;
                case ESDEvent.PropertyInspectorDidAppear: PropertyInspectorAppeared?.Invoke(this, message as ESDPropertyInspectorAppearanceActionMessage); break;
                case ESDEvent.PropertyInspectorDidDisappear: PropertyInspectorDisappeared?.Invoke(this, message as ESDPropertyInspectorAppearanceActionMessage); break;
            }
        }

        public async Task SetTitle(string title, string context, ESDSDKTarget target)
        {
            TitleMessage msg = new TitleMessage { Context = context, Event = ESDFunction.SetTitle, Payload = new ESDTitle { Target = target, Title = title } };
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

            ImageMessage msg = new ImageMessage { Context = context, Event = ESDFunction.SetImage, Payload = new ESDImage { Target = target, Image = base64ImageString } };
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
            AlertMessage msg = new AlertMessage {Context = context, Event = ESDFunction.ShowAlert};

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter<ESDFunction>(JsonNamingPolicy.CamelCase) }
            };
            byte[] buffer = Serialize(msg, options);
            await WebSocket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationTokenSource.Token);
        }

        public async Task ShowOKForContext(string context)
        {
            OkMessage msg = new OkMessage {Context = context, Event = ESDFunction.ShowOk};

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
            SettingsMessage msg = new SettingsMessage { Context = context, Event = ESDFunction.SetSettings, Payload = settings};

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
            StateMessage msg = new StateMessage{Context = context, Event = ESDFunction.SetState, Payload = new ESDState{State = state}};

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
            PropertyInspectorMessage msg = new PropertyInspectorMessage{Context = context, Action = action, Event = ESDFunction.SendToPropertyInspector, Payload = payload};

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

            SwitchProfileMessage msg = new SwitchProfileMessage{Context = PluginUUID, Device = deviceId, Event = ESDFunction.SwitchToProfile, Payload = new ESDProfile{Profile = profileName}};

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
            LogMessage msg = new LogMessage{Event = ESDFunction.LogMessage, Payload = new ESDLogMessage{Message = message}};
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



    public class ConnectMessage
    {
        public string Event { get; set; }
        [JsonPropertyName("uuid")]
        public string UUID { get; set; }
    }

    public class TitleMessage
    {
        public ESDFunction Event { get; set; }

        public string Context { get; set; }

        public ESDTitle Payload { get; set; }
    }

    public class ESDTitle
    {
        public ESDSDKTarget Target { get; set; }

        public string Title { get; set; }
    }

    public class ImageMessage
    {
        public ESDFunction Event { get; set; }

        public string Context { get; set; }

        public ESDImage Payload { get; set; }
    }

    public class ESDImage
    { 
        public ESDSDKTarget Target { get; set; }

        public string Image { get; set; }
    }

    public class AlertMessage
    {
        public ESDFunction Event { get; set; }

        public string Context { get; set; }
    }

    public class OkMessage
    {
        public ESDFunction Event { get; set; }

        public string Context { get; set; }
    }

    public class SettingsMessage
    {
        public ESDFunction Event { get; set; }

        public string Context { get; set; }

        public JsonElement Payload { get; set; }
    }

    public class StateMessage
    {
        public ESDFunction Event { get; set; }

        public string Context { get; set; }

        public ESDState Payload { get; set; }
    }

    public class LogMessage
    {
        public ESDFunction Event { get; set; }

        public ESDLogMessage Payload { get; set; }
    }

    public class ESDState
    {
        public int State { get; set; }
    }

    public class PropertyInspectorMessage
    {
        public ESDFunction Event { get; set; }

        public string Context { get; set; }

        public string Action { get; set; }

        public JsonElement Payload { get; set; }
    }

    public class SwitchProfileMessage
    {
        public ESDFunction Event { get; set; }

        public string Context { get; set; }

        public string Device { get; set; }

        public ESDProfile Payload { get; set; }
    }

    public class ESDProfile
    {
        public string Profile { get; set; }
    }

    public class ESDLogMessage
    {
        public string Message { get; set; }
    }
    public class JsonStringEnumConverter<T> : JsonConverterFactory
    {
        private JsonStringEnumConverter Converter { get; }

        public JsonStringEnumConverter(JsonNamingPolicy namingPolicy = null, bool allowIntegers = true)
        {
            Converter = new JsonStringEnumConverter(namingPolicy, allowIntegers);
        }

        public override bool CanConvert(Type typeToConvert)
        {
            return Converter.CanConvert(typeToConvert) && typeof(T) == typeToConvert;
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return Converter.CreateConverter(typeToConvert, options);
        }
    }
}
