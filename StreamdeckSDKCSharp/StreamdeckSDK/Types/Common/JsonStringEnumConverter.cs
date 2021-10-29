using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Elgato.StreamdeckSDK.Types.Common
{
    public class JsonStringEnumConverter<T> : JsonConverterFactory
    {
        private JsonStringEnumConverter Converter { get; }

        public JsonStringEnumConverter(JsonNamingPolicy namingPolicy = null, bool allowIntegers = true)
        {
            Converter = new JsonStringEnumConverter(namingPolicy, allowIntegers);
        }

        public override bool CanConvert(Type typeToConvert)
        {
            return Converter.CanConvert(typeToConvert) && typeof(T) == typeToConvert;
        }

        public override JsonConverter CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return Converter.CreateConverter(typeToConvert, options);
        }
    }
}
