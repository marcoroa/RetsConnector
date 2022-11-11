namespace CrestApps.RetsSdk.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class TooManyOutstandingRequestsException : Exception
    {
        private const string DefaultMessage = "Too many outstanding requests";

        public TooManyOutstandingRequestsException()
            : base(DefaultMessage)
        {
        }

        public TooManyOutstandingRequestsException(string message)
            : base(message)
        {
        }

        protected TooManyOutstandingRequestsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
