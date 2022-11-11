namespace CrestApps.RetsSdk.Services
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using CrestApps.RetsSdk.Exceptions;
    using CrestApps.RetsSdk.Helpers;
    using CrestApps.RetsSdk.Models;
    using CrestApps.RetsSdk.Models.Enums;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class RetsSession : RetsResponseBase<RetsSession>, IRetsSession
    {
        protected readonly IRetsRequester RetsRequester;
        protected readonly ConnectionOptions Options;

        protected Uri LoginUri => new Uri(this.Options.LoginUrl);
        protected Uri LogoutUri => this.Resource.GetCapability(Capability.Logout);

        public RetsSession(ILogger<RetsSession> logger, IRetsRequester retsRequester, IOptions<ConnectionOptions> connectionOptions)
            : base(logger)
        {
            this.RetsRequester = retsRequester;
            this.Options = connectionOptions.Value;
        }

        private SessionResource _Resource;

        public SessionResource Resource
        {
            get
            {
                return this._Resource ?? throw new Exception("Session is not yet started. Please login first.");
            }
        }

        public async Task<bool> Start()
        {
            this._Resource = await this.RetsRequester.Get(this.LoginUri, async (response) =>
            {
                using (Stream stream = await this.GetStream(response))
                {
                    XDocument doc = XDocument.Load(stream);

                    this.AssertValidReplay(doc.Root);

                    XNamespace ns = doc.Root.GetDefaultNamespace();

                    XElement element = doc.Descendants(ns + "RETS-RESPONSE").FirstOrDefault()
                    ?? throw new RetsParsingException("Unable to find the RETS-RESPONSE element in the response.");

                    var parts = element.FirstNode.ToString().Split(Environment.NewLine);
                    var cookie = response.Headers.GetValues("Set-Cookie").FirstOrDefault();

                    return this.GetRetsResource(parts, cookie);
                }
            });

            return this.IsStarted();
        }

        public async Task End()
        {
            await this.RetsRequester.Get(this.LogoutUri, this._Resource);

            this._Resource = null;
        }

        protected SessionResource GetRetsResource(string[] parts, string cookie)
        {
            var resource = new SessionResource()
            {
                SessionId = this.MakeRetsSessionId(cookie),
                Cookie = cookie,
            };

            foreach (var part in parts)
            {
                if (!part.Contains("="))
                {
                    continue;
                }

                var line = part.Split('=');

                if (Enum.TryParse(line[0].Trim(), out Capability result))
                {
                    resource.AddCapability(result, $"{this.Options.BaseUrl}{line[1].Trim()}", this.Options.UriType);
                }
            }

            return resource;
        }

        private string MakeRetsSessionId(string cookie)
        {
            string sessionId = this.ExtractSessionId(cookie);

            if (string.IsNullOrWhiteSpace(sessionId))
            {
                return null;
            }

            string agentData = Str.Md5(this.Options.UserAgent + ":" + this.Options.UserAgentPassward);

            return $"{agentData}::{sessionId}:{this.Options.Version.AsHeader()}";
        }

        protected string ExtractSessionId(string cookie)
        {
            var parts = cookie.Split(';');

            foreach (var part in parts)
            {
                string cleanedPart = part.Trim();

                if (!cleanedPart.StartsWith("RETS-Session-ID", StringComparison.CurrentCultureIgnoreCase) || !cleanedPart.Contains("="))
                {
                    continue;
                }

                var line = cleanedPart.Split('=');

                return line[1];
            }

            return null;
        }

        public bool IsStarted()
        {
            return this._Resource != null;
        }
    }
}
