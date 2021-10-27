using System;
using System.Collections.Generic;
using Elgato.StreamdeckSDK.Types.Messages;
using Elgato.StreamdeckSDK.Types.Messages.ESDActions;

namespace Elgato.StreamdeckSDK.Types.Common
{
    internal static class ESDEventMap
    {
        internal static Dictionary<ESDEvent, Type> EventMap { get; }
            = new Dictionary<ESDEvent, Type>
            {
                {ESDEvent.KeyUp, typeof(ESDKeyActionMessage)},
                {ESDEvent.KeyDown, typeof(ESDKeyActionMessage)},
                {ESDEvent.WillAppear, typeof(ESDAppearanceActionMessage)},
                {ESDEvent.WillDisappear, typeof(ESDAppearanceActionMessage)},
                {ESDEvent.DeviceDidConnect, typeof(ESDDeviceConnectMessage)},
                {ESDEvent.DeviceDidDisconnect, typeof(ESDDeviceDisconnectMessage)},
                {ESDEvent.ApplicationDidLaunch, typeof(ESDApplicationMessage)},
                {ESDEvent.ApplicationDidTerminate, typeof(ESDApplicationMessage)},
                {ESDEvent.TitleParametersDidChange, typeof(ESDTitleParametersChangeActionMessage)},
                {ESDEvent.SendToPlugin, typeof(ESDSendToPluginActionMessage)},
                {ESDEvent.DidReceiveSettings, typeof(ESDSettingsActionMessage)},
                {ESDEvent.DidReceiveGlobalSettings, typeof(ESDGlobalSettingsMessage)},
                {ESDEvent.PropertyInspectorDidAppear, typeof(ESDPropertyInspectorAppearanceActionMessage)},
                {ESDEvent.PropertyInspectorDidDisappear, typeof(ESDPropertyInspectorAppearanceActionMessage)},
            };
    }
}
