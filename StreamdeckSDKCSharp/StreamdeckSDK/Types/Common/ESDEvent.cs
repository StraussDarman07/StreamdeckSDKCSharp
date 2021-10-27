namespace Elgato.StreamdeckSDK.Types.Common
{
    public enum ESDEvent
    {
        KeyUp,
        KeyDown,
        SendToPlugin,
        WillAppear,
        WillDisappear,
        DeviceDidConnect,
        DeviceDidDisconnect,
        ApplicationDidLaunch,
        ApplicationDidTerminate,
        //SystemDidWakeUp,
        TitleParametersDidChange,
        DidReceiveSettings,
        DidReceiveGlobalSettings,
        PropertyInspectorDidAppear,
        PropertyInspectorDidDisappear,


    }

    public enum ESDFunction
    {
        SetTitle,
        SetImage,
        ShowAlert,
        ShowOk,
        SetSettings,
        SetState,
        SendToPropertyInspector,
        SwitchToProfile,
        LogMessage,
    }
}
