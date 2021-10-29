using Elgato.StreamdeckSDK.Types.Common;

namespace Elgato.StreamdeckSDK.Types.Messages
{
    public abstract class ESDMessage
    {   
        public ESDFunction Event { get; set; }
    }
}
