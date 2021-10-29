using System.Text.Json;
using Elgato.StreamdeckSDK.Types.Payloads.Messages;

namespace Elgato.StreamdeckSDK.Types.Messages.ContextualMessages
{
    public abstract class ESDContextualPayloadMessage<T> : ESDContextualMessage
    {
        public T Payload { get; set; }
    }

    public class ESDTitleMessage : ESDContextualPayloadMessage<ESDTitleMessagePayload> {}

    public class ESDImageMessage : ESDContextualPayloadMessage<ESDImageMessagePayload>{}

    public class ESDSettingsMessage : ESDContextualPayloadMessage<JsonElement> {}

    public class ESDStateMessage : ESDContextualPayloadMessage<ESDStateMessagePayload> {}

    public class ESDSwitchProfileMessage : ESDContextualPayloadMessage<ESDProfileMessagePayload> {public string Device { get; set; }}

    public class ESDPropertyInspectorMessage : ESDContextualPayloadMessage<JsonElement> {public string Action { get; set; }}
}
