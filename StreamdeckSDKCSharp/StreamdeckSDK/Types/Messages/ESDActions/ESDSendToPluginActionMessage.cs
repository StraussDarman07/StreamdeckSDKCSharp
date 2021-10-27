using System.Text.Json;

namespace Elgato.StreamdeckSDK.Types.Messages.ESDActions
{
    public class ESDSendToPluginActionMessage : ESDActionMessage
    {
        public JsonElement Payload { get; set; }
    }
}
