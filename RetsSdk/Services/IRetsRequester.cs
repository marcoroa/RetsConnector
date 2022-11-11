namespace CrestApps.RetsSdk.Services
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using CrestApps.RetsSdk.Models;

    public interface IRetsRequester
    {
        Task Get(Uri uri, Action<HttpResponseMessage> action, SessionResource resource = null, bool ensureSuccessStatusCode = true);
        Task<T> Get<T>(Uri uri, Func<HttpResponseMessage, Task<T>> action, SessionResource resource = null, bool ensureSuccessStatusCode = true)
            where T : class;
        Task Get(Uri uri, SessionResource resource = null, bool ensureSuccessStatusCode = true);
    }
}
