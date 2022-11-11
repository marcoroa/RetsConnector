namespace CrestApps.RetsSdk.Exceptions
{
    using System;

    public class TooManyOutstandingQueries : Exception
    {
        public TooManyOutstandingQueries()
            : base("Too many outstanding queries")
        {
        }

        public TooManyOutstandingQueries(string message)
            : base(message)
        {
        }
    }
}
