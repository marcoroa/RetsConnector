namespace CrestApps.RetsSdk.Services
{
    using System;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using CrestApps.RetsSdk.Exceptions;
    using Microsoft.Extensions.Logging;

    public abstract class RetsResponseBase<T>
    {
        protected readonly ILogger<T> Log;

        public RetsResponseBase(ILogger<T> log)
        {
            this.Log = log;
        }

        protected async Task<Stream> GetStream(HttpResponseMessage response)
        {
            Stream memoryStream = await response.Content.ReadAsStreamAsync();

            if (response.Content.Headers.ContentEncoding.Any(x => x.Equals("gzip", StringComparison.CurrentCultureIgnoreCase)))
            {
                return new GZipStream(memoryStream, CompressionMode.Decompress);
            }

            return memoryStream;
        }

        protected int GetReplayCode(XElement root)
        {
            var replayCode = root.Attribute("ReplyCode");

            if (replayCode != null && int.TryParse(replayCode.Value, out int code))
            {
                return code;
            }

            if (replayCode == null)
            {
                this.Log?.LogError("Unable to find ReplyCode attribute on the XElement.");
            }

            return int.MaxValue;
        }

        protected void AssertValidReplay(XElement root)
        {
            int code = this.GetReplayCode(root);

            this.AssertValidReplay(root, code);
        }

        protected void AssertValidReplay(XElement root, int code)
        {
            if (!this.IsValidCode(code))
            {
                var replayText = root.Attribute("ReplyText");
                string message = replayText?.Value ?? "Unknown error";

                this.Log?.LogWarning(message);

                if (code == 20210)
                {
                    throw new TooManyOutstandingQueries(message);
                }

                if (code == 20512 || code == 20412)
                {
                    throw new TooManyOutstandingRequests(message);
                }

                throw new RetsException(message);
            }
        }

        protected bool IsValidCode(int code)
        {
            // 20201 - No records found.
            // 20403 - No objects found
            // 0 - Success

            return code == 0 || code == 20201 || code == 20403;
        }
    }
}
