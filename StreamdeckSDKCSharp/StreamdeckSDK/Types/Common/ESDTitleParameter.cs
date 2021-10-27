using System.Text.Json;
using System.Text.Json.Serialization;

namespace Elgato.StreamdeckSDK.Types.Common
{
    public class ESDTitleParameter
    {
        private const string COORDINATES_KEY = "coordinates";
        private const string FONT_STYLE_KEY = "titleParameters";

        public int State { get; set; }

        public string Title { get; set; }

        [JsonPropertyName(COORDINATES_KEY)]
        public ESDCoordinate Coordinate { get; set; }

        [JsonPropertyName(FONT_STYLE_KEY)]
        public ESDFontStyle FontStyle { get; set; }

        public JsonElement Settings { get; set; }
    }
}
