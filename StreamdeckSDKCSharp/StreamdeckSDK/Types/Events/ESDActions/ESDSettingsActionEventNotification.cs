using Elgato.StreamdeckSDK.Types.Common;

namespace Elgato.StreamdeckSDK.Types.Events.ESDActions
{
    public class ESDSettingsActionEventNotification : ESDActionEventNotification
    {
        public ESDSettings Payload { get; set; }
    }
}
