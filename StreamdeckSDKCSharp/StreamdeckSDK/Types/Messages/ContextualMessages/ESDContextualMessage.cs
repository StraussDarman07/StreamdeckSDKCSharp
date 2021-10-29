namespace Elgato.StreamdeckSDK.Types.Messages.ContextualMessages
{
    public abstract class ESDContextualMessage : ESDMessage
    {
        public string Context { get; set; }
    }

    public class ESDOkMessage : ESDContextualMessage {}
    public class ESDAlertMessage : ESDContextualMessage {}
}
