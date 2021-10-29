using Elgato.StreamdeckSDK.Types.Common;

namespace Elgato.StreamdeckSDK.Types.Events
{
    public class ESDDeviceConnectEventNotification : ESDEventNotification
    {
        public ESDDeviceInfo DeviceInfo { get; set; }
    }
}
