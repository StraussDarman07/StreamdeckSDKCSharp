using Elgato.StreamdeckSDK.Types.Common;

namespace Elgato.StreamdeckSDK.Types.Messages.ESDActions
{
    public class ESDAppearanceActionMessage : ESDActionMessage
    {
        public ESDAppearance Payload { get; set; }
    }
}
