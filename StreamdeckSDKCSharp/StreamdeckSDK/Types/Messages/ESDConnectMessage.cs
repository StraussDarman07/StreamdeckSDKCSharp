using System.Text.Json.Serialization;

namespace Elgato.StreamdeckSDK.Types.Messages
{
    public class ESDConnectMessage
    {
        public string Event { get; set; }

        [JsonPropertyName("uuid")]
        public string UUID { get; set; }
    }
}
