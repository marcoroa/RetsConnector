namespace CrestApps.RetsSdk.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class RetsParsingException : Exception
    {
        private const string DefaultMessage = "Unable to parse the response";

        public RetsParsingException()
            : base(DefaultMessage)
        {
        }

        public RetsParsingException(string message)
            : base(message)
        {
        }

        protected RetsParsingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
