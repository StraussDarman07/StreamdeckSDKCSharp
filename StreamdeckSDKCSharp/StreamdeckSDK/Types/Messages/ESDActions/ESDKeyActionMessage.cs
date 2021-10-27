using Elgato.StreamdeckSDK.Types.Payloads;

namespace Elgato.StreamdeckSDK.Types.Messages.ESDActions
{
    public class ESDKeyActionMessage : ESDActionMessage
    {
        public ESDKeyPayload Payload { get; set; }
    }
}
