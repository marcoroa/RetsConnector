namespace CrestApps.RetsSdk.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class VersionParsingException : Exception
    {
        private const string DefaultMessage = "The given version is not valid. valid version should in the following format 'Number.Number.Number' ";

        public VersionParsingException()
            : base(DefaultMessage)
        {
        }

        public VersionParsingException(string message)
            : base(message)
        {
        }

        protected VersionParsingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
