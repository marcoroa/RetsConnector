namespace CrestApps.RetsSdk.Services
{
    using System.Threading.Tasks;
    using CrestApps.RetsSdk.Models;

    public interface IRetsSession
    {
        Task<bool> Start();
        Task End();

        SessionResource Resource { get; }
        bool IsStarted();
    }
}
