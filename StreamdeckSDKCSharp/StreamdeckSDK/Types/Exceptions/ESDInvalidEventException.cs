using System;
using System.Runtime.Serialization;

namespace Elgato.StreamdeckSDK.Types.Exceptions
{
    [Serializable]
    public class ESDInvalidEventException : ESDSDKException
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public ESDInvalidEventException()
        {
        }

        public ESDInvalidEventException(string message) : base(message)
        {
        }

        public ESDInvalidEventException(string message, System.Exception inner) : base(message, inner)
        {
        }

        protected ESDInvalidEventException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
