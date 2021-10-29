using Elgato.StreamdeckSDK.Types.Common;

namespace Elgato.StreamdeckSDK.Types.Events
{
    public class ESDGlobalSettingsEventNotification : ESDEventNotification
    {
        public ESDGlobalSettings Payload { get; set; }
    }
}
