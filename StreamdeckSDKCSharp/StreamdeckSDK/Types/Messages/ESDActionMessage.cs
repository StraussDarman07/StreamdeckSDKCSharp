using Elgato.StreamdeckSDK.Types.Common;
using Elgato.StreamdeckSDK.Types.Payloads;

namespace Elgato.StreamdeckSDK.Types.Messages
{
    public abstract class ESDActionMessage : ESDMessage
    {
        public string Action { get; set; }

        public string Context { get; set; }
    }
}
