namespace CrestApps.RetsSdk.Services
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using CrestApps.RetsSdk.Models;
    using Microsoft.Extensions.Options;

    public class RetsWebRequester : IRetsRequester
    {
        private readonly ConnectionOptions options;
        private readonly IHttpClientFactory httpClientFactory;

        public RetsWebRequester(IOptions<ConnectionOptions> options, IHttpClientFactory httpClientFactory)
        {
            this.options = options.Value;
            this.httpClientFactory = httpClientFactory;
        }

        public async Task Get(Uri uri, Action<HttpResponseMessage> action, SessionResource resource = null, bool ensureSuccessStatusCode = true)
        {
            using (var client = this.GetClient(resource))
            {
                var response = await client.GetAsync(uri);

                if (ensureSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                }

                action?.Invoke(response);
            }
        }

        public async Task<T> Get<T>(Uri uri, Func<HttpResponseMessage, Task<T>> action, SessionResource resource = null, bool ensureSuccessStatusCode = true)
            where T : class
        {
            using (var client = this.GetClient(resource))
            {
                var response = await client.GetAsync(uri);

                if (ensureSuccessStatusCode)
                {
                    response.EnsureSuccessStatusCode();
                }

                return await action?.Invoke(response);
            }
        }

        public async Task Get(Uri uri, SessionResource resource = null, bool ensureSuccessStatusCode = true)
        {
            await this.Get(uri, null, resource, ensureSuccessStatusCode);
        }

        protected virtual HttpClient GetClient(SessionResource resource)
        {
            HttpClient client = this.GetAuthenticatedClient();

            client.Timeout = this.options.Timeout;
            client.DefaultRequestHeaders.Add("User-Agent", this.options.UserAgent);
            client.DefaultRequestHeaders.Add("RETS-Version", this.options.Version.AsHeader());
            client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip");
            client.DefaultRequestHeaders.Add("Accept", "*/*");

            if (resource != null && !string.IsNullOrWhiteSpace(resource.Cookie))
            {
                client.DefaultRequestHeaders.Add("Set-Cookie", resource.Cookie);
            }

            if (resource != null && !string.IsNullOrWhiteSpace(resource.SessionId))
            {
                client.DefaultRequestHeaders.Add("RETS-Session-ID", resource.SessionId);
            }

            return client;
        }

        private HttpClient GetAuthenticatedClient()
        {
            if (this.options.Type == Models.Enums.AuthenticationType.Digest)
            {
                var credCache = new CredentialCache();
                credCache.Add(new Uri(this.options.LoginUrl), this.options.Type.ToString(), new NetworkCredential(this.options.Username, this.options.Password));

                return new HttpClient(new HttpClientHandler { Credentials = credCache });
            }

            HttpClient client = this.httpClientFactory.CreateClient();

            byte[] byteArray = Encoding.ASCII.GetBytes($"{this.options.Username}:{this.options.Password}");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

            return client;
        }
    }
}
