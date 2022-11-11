namespace CrestApps.RetsSdk.Exceptions
{
    using System;

    public class ResourceDoesNotExists : Exception
    {
        public ResourceDoesNotExists()
            : base("The given resource does not exists.")
        {
        }
    }
}
