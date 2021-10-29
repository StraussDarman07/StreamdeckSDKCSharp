using System;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Elgato.StreamdeckSDK.Types.Common;
using Elgato.StreamdeckSDK.Types.Exceptions;

namespace Elgato.StreamdeckSDK.Types.Events
{
    public abstract class ESDEventNotification
    {
        private const string EVENT_KEY = "event";
        private const string DEVICE_KEY = "device";

        [JsonPropertyName(EVENT_KEY)]
        public Common.ESDEvent Event { get; set; }
        [JsonPropertyName(DEVICE_KEY)]
        public string DeviceId { get; set; }

        private static JsonSerializerOptions JsonOptions { get; }
            = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };

        private static MethodInfo GenericDeserializer { get; }
            = typeof(JsonSerializer)
                .GetMethods()
                .Where(m => m.Name == "Deserialize")
                .Select(m => new
                {
                    Method = m,
                    Args = m.GetGenericArguments(),
                    Return = m.ReturnParameter
                })
                .Where(x => x.Args.Length == 1
                && x.Return.ParameterType == x.Args[0])
                .Select(x => x.Method)
                .First();

        public static ESDEventNotification Parse(Memory<byte> memory)
        {
            JsonDocument document = JsonDocument.Parse(memory);
            string json = Encoding.UTF8.GetString(memory.Span);
            JsonElement rootElement = document.RootElement;

            if (!rootElement.TryGetProperty(EVENT_KEY, out JsonElement eventElement))
            {
                throw new ESDInvalidEventException(json);
            }

            if (!Enum.TryParse(eventElement.GetString(), true, out Common.ESDEvent streamdeckEvent))
            {
                throw new ESDInvalidEventException(json);
            }


            MethodInfo deserializer = GenericDeserializer.MakeGenericMethod(ESDEventMap.EventMap[streamdeckEvent]);

            if (!(deserializer.Invoke(null, new object?[]{json, JsonOptions}) is ESDEventNotification message))
            {
                throw new ESDJsonParseFailedException();
            }

            return message;
        }
    }
}
