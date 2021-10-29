using System.Text.Json;

namespace Elgato.StreamdeckSDK.Types.Events.ESDActions
{
    public class ESDSendToPluginActionEventNotification : ESDActionEventNotification
    {
        public JsonElement Payload { get; set; }
    }
}
