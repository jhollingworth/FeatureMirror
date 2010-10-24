using System;
using System.Runtime.Serialization;

namespace FeatureMirror
{
    [Serializable]
    public class UnknownDestinationException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public UnknownDestinationException()
        {
        }

        public UnknownDestinationException(string message) : base(message)
        {
        }

        public UnknownDestinationException(string message, Exception inner) : base(message, inner)
        {
        }

        protected UnknownDestinationException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}