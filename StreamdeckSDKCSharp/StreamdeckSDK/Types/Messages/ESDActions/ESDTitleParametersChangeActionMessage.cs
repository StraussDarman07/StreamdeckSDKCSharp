using Elgato.StreamdeckSDK.Types.Common;

namespace Elgato.StreamdeckSDK.Types.Messages.ESDActions
{
    public class ESDTitleParametersChangeActionMessage : ESDActionMessage
    {
        public ESDTitleParameter Payload { get; set; }
    }
}
