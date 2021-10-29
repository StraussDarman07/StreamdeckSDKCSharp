using Elgato.StreamdeckSDK.Types.Common;
using Elgato.StreamdeckSDK.Types.Payloads;

namespace Elgato.StreamdeckSDK.Types.Events
{
    public abstract class ESDActionEventNotification : ESDEventNotification
    {
        public string Action { get; set; }

        public string Context { get; set; }
    }
}
