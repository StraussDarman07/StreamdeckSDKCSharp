using System;
using System.Runtime.Serialization;

namespace Elgato.StreamdeckSDK.Types.Exceptions
{
    [Serializable]
    public class ESDJsonParseFailedException : ESDSDKException
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public ESDJsonParseFailedException()
        {
        }

        public ESDJsonParseFailedException(string message) : base(message)
        {
        }

        public ESDJsonParseFailedException(string message, System.Exception inner) : base(message, inner)
        {
        }

        protected ESDJsonParseFailedException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
