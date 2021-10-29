using System.Text.Json;
using System.Text.Json.Serialization;
using Elgato.StreamdeckSDK.Types.Common;

namespace Elgato.StreamdeckSDK.Types.Payloads.Events
{
    public abstract class ESDEventPayload
    {
        private const string COORDINATE_KEY = "coordinates";

        [JsonPropertyName(COORDINATE_KEY)]
        public ESDCoordinate Coordinate { get; set; }

        public bool IsInMultiAction { get; set; }

        public JsonElement Settings { get; set; }
    }
}
