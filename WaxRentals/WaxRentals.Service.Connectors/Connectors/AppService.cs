using System;
using System.Threading.Tasks;
using WaxRentals.Service.Shared.Entities;

namespace WaxRentals.Service.Shared.Connectors
{
    public interface IAppService
    {
        Task<Result<AppState>> State();
    }

    internal class AppService : Connector, IAppService
    {

        public AppService(Uri baseUrl, ITrackService log) : base(baseUrl, log) { }

        public async Task<Result<AppState>> State()
        {
            return await Get<AppState>("State");
        }

    }
}
