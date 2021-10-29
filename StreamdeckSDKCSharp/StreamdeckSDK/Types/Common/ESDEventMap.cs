using System;
using System.Collections.Generic;
using Elgato.StreamdeckSDK.Types.Events;
using Elgato.StreamdeckSDK.Types.Events.ESDActions;

namespace Elgato.StreamdeckSDK.Types.Common
{
    internal static class ESDEventMap
    {
        internal static Dictionary<ESDEvent, Type> EventMap { get; }
            = new Dictionary<ESDEvent, Type>
            {
                {ESDEvent.KeyUp, typeof(ESDKeyActionEventNotification)},
                {ESDEvent.KeyDown, typeof(ESDKeyActionEventNotification)},
                {ESDEvent.WillAppear, typeof(ESDAppearanceActionEventNotification)},
                {ESDEvent.WillDisappear, typeof(ESDAppearanceActionEventNotification)},
                {ESDEvent.DeviceDidConnect, typeof(ESDDeviceConnectEventNotification)},
                {ESDEvent.DeviceDidDisconnect, typeof(ESDDeviceDisconnectEventNotification)},
                {ESDEvent.ApplicationDidLaunch, typeof(ESDApplicationEventNotification)},
                {ESDEvent.ApplicationDidTerminate, typeof(ESDApplicationEventNotification)},
                {ESDEvent.TitleParametersDidChange, typeof(ESDTitleParametersChangeActionEventNotification)},
                {ESDEvent.SendToPlugin, typeof(ESDSendToPluginActionEventNotification)},
                {ESDEvent.DidReceiveSettings, typeof(ESDSettingsActionEventNotification)},
                {ESDEvent.DidReceiveGlobalSettings, typeof(ESDGlobalSettingsEventNotification)},
                {ESDEvent.PropertyInspectorDidAppear, typeof(ESDPropertyInspectorAppearanceActionEventNotification)},
                {ESDEvent.PropertyInspectorDidDisappear, typeof(ESDPropertyInspectorAppearanceActionEventNotification)},
            };
    }
}
