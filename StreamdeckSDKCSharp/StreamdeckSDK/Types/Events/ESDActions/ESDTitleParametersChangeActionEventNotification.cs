using Elgato.StreamdeckSDK.Types.Common;

namespace Elgato.StreamdeckSDK.Types.Events.ESDActions
{
    public class ESDTitleParametersChangeActionEventNotification : ESDActionEventNotification
    {
        public ESDTitleParameter Payload { get; set; }
    }
}
