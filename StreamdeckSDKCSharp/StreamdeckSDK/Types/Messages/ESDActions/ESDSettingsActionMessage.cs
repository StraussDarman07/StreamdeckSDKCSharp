using Elgato.StreamdeckSDK.Types.Common;

namespace Elgato.StreamdeckSDK.Types.Messages.ESDActions
{
    public class ESDSettingsActionMessage : ESDActionMessage
    {
        public ESDSettings Payload { get; set; }
    }
}
