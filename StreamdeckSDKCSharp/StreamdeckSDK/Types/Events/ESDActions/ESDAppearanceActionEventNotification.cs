using Elgato.StreamdeckSDK.Types.Common;

namespace Elgato.StreamdeckSDK.Types.Events.ESDActions
{
    public class ESDAppearanceActionEventNotification : ESDActionEventNotification
    {
        public ESDAppearance Payload { get; set; }
    }
}
