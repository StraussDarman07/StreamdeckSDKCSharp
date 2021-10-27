using Elgato.StreamdeckSDK.Types.Common;

namespace Elgato.StreamdeckSDK.Types.Messages
{
    public class ESDDeviceConnectMessage : ESDMessage
    {
        public ESDDeviceInfo DeviceInfo { get; set; }
    }
}
