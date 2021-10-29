using Elgato.StreamdeckSDK.Types.Payloads.Messages;

namespace Elgato.StreamdeckSDK.Types.Messages
{
    public class ESDLogMessage : ESDMessage
    {
        public ESDLogMessagePayload Payload { get; set; }
    }
}
