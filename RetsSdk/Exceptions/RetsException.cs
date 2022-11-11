namespace CrestApps.RetsSdk.Exceptions
{
    using System;

    public class RetsException : Exception
    {
        public RetsException()
             : base("Rets server throw an unknow error")
        {
        }

        public RetsException(string message)
             : base(message)
        {
        }
    }
}
