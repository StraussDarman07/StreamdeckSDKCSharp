using Elgato.StreamdeckSDK.Types.Common;

namespace Elgato.StreamdeckSDK.Types.Payloads.Messages
{
    public abstract class ESDMessagePayload
    {
        public ESDSDKTarget Target { get; set; }
    }
}
