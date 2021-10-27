using System.Text.Json;
using System.Text.Json.Serialization;

namespace Elgato.StreamdeckSDK.Types.Common
{
    public class ESDSettings
    {
        private const string COORDINATES_KEY = "coordinates";

        public bool IsInMultiAction { get; set; }

        [JsonPropertyName(COORDINATES_KEY)]
        public ESDCoordinate Coordinate { get; set; }

        public JsonElement Settings { get; set; }
    }
}
