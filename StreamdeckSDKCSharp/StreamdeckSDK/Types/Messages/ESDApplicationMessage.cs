using Elgato.StreamdeckSDK.Types.Common;

namespace Elgato.StreamdeckSDK.Types.Messages
{
    public class ESDApplicationMessage : ESDMessage
    {
        public ESDApplication Payload { get; set; }
    }
}
