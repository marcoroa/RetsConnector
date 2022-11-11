namespace CrestApps.RetsSdk.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class ResourceDoesNotExistsException : Exception
    {
        private const string DefaultMessage = "Unable to parse the response";

        public ResourceDoesNotExistsException()
            : base(DefaultMessage)
        {
        }

        public ResourceDoesNotExistsException(string message)
            : base(message)
        {
        }

        protected ResourceDoesNotExistsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
