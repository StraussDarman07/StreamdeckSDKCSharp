using Elgato.StreamdeckSDK.Types.Common;

namespace Elgato.StreamdeckSDK.Types.Messages
{
    public class ESDGlobalSettingsMessage : ESDMessage
    {
        public ESDGlobalSettings Payload { get; set; }
    }
}
