using Elgato.StreamdeckSDK.Types.Payloads.Events;

namespace Elgato.StreamdeckSDK.Types.Events.ESDActions
{
    public class ESDKeyActionEventNotification : ESDActionEventNotification
    {
        public ESDEventKeyPayload Payload { get; set; }
    }
}
