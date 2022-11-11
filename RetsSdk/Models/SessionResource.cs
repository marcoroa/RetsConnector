namespace CrestApps.RetsSdk.Models
{
    using System;
    using System.Collections.Generic;
    using CrestApps.RetsSdk.Exceptions;
    using CrestApps.RetsSdk.Models.Enums;

    public class SessionResource
    {
        public string SessionId { get; set; }
        public string Cookie { get; set; }

        public Dictionary<Capability, Uri> Capabilities { get; set; }

        public SessionResource()
        {
            this.Capabilities = new Dictionary<Capability, Uri>();
        }

        public void AddCapability(Capability name, string url, UriKind uriType)
        {
            var uri = new Uri(url);

            if (this.Capabilities.ContainsKey(name) || !uri.IsWellFormedOriginalString())
            {
                return;
            }

            this.Capabilities.TryAdd(name, uri);
        }

        public Uri GetCapability(Capability name)
        {
            if (!this.Capabilities.ContainsKey(name))
            {
                throw new MissingCapabilityException();
            }

            return this.Capabilities[name];
        }
    }
}
