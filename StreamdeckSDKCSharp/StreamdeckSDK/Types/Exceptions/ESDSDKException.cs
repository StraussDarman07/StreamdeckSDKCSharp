using System;
using System.Runtime.Serialization;

namespace Elgato.StreamdeckSDK.Types.Exceptions
{
    [Serializable]
    public class ESDSDKException : System.Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public ESDSDKException()
        {
        }

        public ESDSDKException(string message) : base(message)
        {
        }

        public ESDSDKException(string message, System.Exception inner) : base(message, inner)
        {
        }

        protected ESDSDKException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
