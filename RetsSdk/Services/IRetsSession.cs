namespace CrestApps.RetsSdk.Services
{
    using System.Threading.Tasks;
    using CrestApps.RetsSdk.Models;

    public interface IRetsSession
    {
        SessionResource Resource { get; }

        Task<bool> Start();
        Task End();
        bool IsStarted();
    }
}
