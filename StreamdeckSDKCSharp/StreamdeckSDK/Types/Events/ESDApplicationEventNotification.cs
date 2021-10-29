using Elgato.StreamdeckSDK.Types.Common;

namespace Elgato.StreamdeckSDK.Types.Events
{
    public class ESDApplicationEventNotification : ESDEventNotification
    {
        public ESDApplication Payload { get; set; }
    }
}
