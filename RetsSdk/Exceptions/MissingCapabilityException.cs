namespace CrestApps.RetsSdk.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class MissingCapabilityException : Exception
    {
        private const string DefaultMessage = "The requested capability does not exists";

        public MissingCapabilityException()
            : base(DefaultMessage)
        {
        }

        public MissingCapabilityException(string message)
            : base(message)
        {
        }

        protected MissingCapabilityException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
