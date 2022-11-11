namespace CrestApps.RetsSdk.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class TooManyOutstandingQueriesException : Exception
    {
        private const string DefaultMessage = "Too many outstanding queries";

        public TooManyOutstandingQueriesException()
            : base(DefaultMessage)
        {
        }

        public TooManyOutstandingQueriesException(string message)
            : base(message)
        {
        }

        protected TooManyOutstandingQueriesException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
