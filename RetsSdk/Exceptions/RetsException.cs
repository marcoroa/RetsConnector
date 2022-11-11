namespace CrestApps.RetsSdk.Exceptions
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class RetsException : Exception
    {
        private const string DefaultMessage = "Rets server throw an unknow error";

        public RetsException()
             : base(DefaultMessage)
        {
        }

        public RetsException(string message)
             : base(message)
        {
        }

        protected RetsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
