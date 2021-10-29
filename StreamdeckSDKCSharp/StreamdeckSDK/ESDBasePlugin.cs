using System;
using Elgato.StreamdeckSDK.Types.Events;
using Elgato.StreamdeckSDK.Types.Events.ESDActions;

namespace Elgato.StreamdeckSDK
{
    public abstract class ESDBasePlugin : IDisposable
    {
        protected ESDConnectionManager ESDConnectionManager { get; }

        protected ESDBasePlugin(ESDConnectionManager connectionManager)
        {
            ESDConnectionManager = connectionManager;
            SubscribeToStreamdeck();
        }

        private void SubscribeToStreamdeck()
        {
            ESDConnectionManager.KeyDownForAction += OnKeyDown;
            ESDConnectionManager.KeyUpForAction += OnKeyUp;

            ESDConnectionManager.SendToPlugin += OnSendToPlugin;

            ESDConnectionManager.DeviceDidDisconnect += OnDeviceDidDisconnect;
            ESDConnectionManager.DeviceDidConnect += OnDeviceDidConnect;

            ESDConnectionManager.ApplicationDidLaunch += OnApplicationDidLaunch;
            ESDConnectionManager.ApplicationDidTerminate += OnApplicationDidTerminate;

            ESDConnectionManager.DidReceiveSettings += OnReceiveSettings;
            ESDConnectionManager.DidReceiveGlobalSettings += OnReceiveGlobalSettings;

            ESDConnectionManager.PropertyInspectorAppeared += OnPropertyInspectorAppeared;
            ESDConnectionManager.PropertyInspectorDisappeared += OnPropertyInspectorDisappeared;

            ESDConnectionManager.TitleParametersChanged += OnTitleParametersChanged;

            ESDConnectionManager.WillAppearForAction += OnWillAppearForAction;
            ESDConnectionManager.WillDisappearForAction += OnWillDisappearForAction;
        }

        private void UnSubscribeFromStreamdeck()
        {
            ESDConnectionManager.KeyDownForAction -= OnKeyDown;
            ESDConnectionManager.KeyUpForAction -= OnKeyUp;

            ESDConnectionManager.SendToPlugin -= OnSendToPlugin;

            ESDConnectionManager.DeviceDidDisconnect -= OnDeviceDidDisconnect;
            ESDConnectionManager.DeviceDidConnect -= OnDeviceDidConnect;

            ESDConnectionManager.ApplicationDidLaunch -= OnApplicationDidLaunch;
            ESDConnectionManager.ApplicationDidTerminate -= OnApplicationDidTerminate;

            ESDConnectionManager.DidReceiveSettings -= OnReceiveSettings;
            ESDConnectionManager.DidReceiveGlobalSettings -= OnReceiveGlobalSettings;

            ESDConnectionManager.PropertyInspectorAppeared -= OnPropertyInspectorAppeared;
            ESDConnectionManager.PropertyInspectorDisappeared -= OnPropertyInspectorDisappeared;

            ESDConnectionManager.TitleParametersChanged -= OnTitleParametersChanged;

            ESDConnectionManager.WillAppearForAction -= OnWillAppearForAction;
            ESDConnectionManager.WillDisappearForAction -= OnWillDisappearForAction;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                UnSubscribeFromStreamdeck();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void OnKeyDown(object sender, ESDKeyActionEventNotification keyAction) {}
        protected virtual void OnKeyUp(object sender, ESDKeyActionEventNotification keyAction) {}
        protected virtual void OnSendToPlugin(object sender, ESDSendToPluginActionEventNotification keyAction) {}
        protected virtual void OnDeviceDidConnect(object sender, ESDDeviceConnectEventNotification keyAction) {}
        protected virtual void OnDeviceDidDisconnect(object sender, ESDDeviceDisconnectEventNotification keyAction) {}
        protected virtual void OnWillDisappearForAction(object sender, ESDAppearanceActionEventNotification e) {}
        protected virtual void OnWillAppearForAction(object sender, ESDAppearanceActionEventNotification e){}
        protected virtual void OnTitleParametersChanged(object sender, ESDTitleParametersChangeActionEventNotification e){}
        protected virtual void OnPropertyInspectorDisappeared(object sender, ESDPropertyInspectorAppearanceActionEventNotification e){}
        protected virtual void OnPropertyInspectorAppeared(object sender, ESDPropertyInspectorAppearanceActionEventNotification e){}
        protected virtual void OnReceiveGlobalSettings(object sender, ESDGlobalSettingsEventNotification e){}
        protected virtual void OnReceiveSettings(object sender, ESDSettingsActionEventNotification e){}
        protected virtual void OnApplicationDidTerminate(object sender, ESDApplicationEventNotification e){}
        protected virtual void OnApplicationDidLaunch(object sender, ESDApplicationEventNotification e){}

    }
}
