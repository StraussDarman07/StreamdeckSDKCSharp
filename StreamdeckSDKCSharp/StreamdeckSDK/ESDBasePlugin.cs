using System;
using Elgato.StreamdeckSDK.Types.Messages;
using Elgato.StreamdeckSDK.Types.Messages.ESDActions;

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

        protected virtual void OnKeyDown(object sender, ESDKeyActionMessage keyAction) {}
        protected virtual void OnKeyUp(object sender, ESDKeyActionMessage keyAction) {}
        protected virtual void OnSendToPlugin(object sender, ESDSendToPluginActionMessage keyAction) {}
        protected virtual void OnDeviceDidConnect(object sender, ESDDeviceConnectMessage keyAction) {}
        protected virtual void OnDeviceDidDisconnect(object sender, ESDDeviceDisconnectMessage keyAction) {}
        protected virtual void OnWillDisappearForAction(object sender, ESDAppearanceActionMessage e) {}
        protected virtual void OnWillAppearForAction(object sender, ESDAppearanceActionMessage e){}
        protected virtual void OnTitleParametersChanged(object sender, ESDTitleParametersChangeActionMessage e){}
        protected virtual void OnPropertyInspectorDisappeared(object sender, ESDPropertyInspectorAppearanceActionMessage e){}
        protected virtual void OnPropertyInspectorAppeared(object sender, ESDPropertyInspectorAppearanceActionMessage e){}
        protected virtual void OnReceiveGlobalSettings(object sender, ESDGlobalSettingsMessage e){}
        protected virtual void OnReceiveSettings(object sender, ESDSettingsActionMessage e){}
        protected virtual void OnApplicationDidTerminate(object sender, ESDApplicationMessage e){}
        protected virtual void OnApplicationDidLaunch(object sender, ESDApplicationMessage e){}

    }
}
