using System;
using System.Threading.Tasks;
using WaxRentals.Service.Shared.Entities;

namespace WaxRentals.Service.Shared.Connectors
{
    public interface IAppService
    {
        Task<Result<AppState>> State();
        Task<Result<AppInsights>> Insights();
    }

    internal class AppService : Connector, IAppService
    {

        public AppService(Uri baseUrl, ITrackService log) : base(baseUrl, log) { }

        public async Task<Result<AppState>> State()
        {
            return await Get<AppState>("State");
        }

        public async Task<Result<AppInsights>> Insights()
        {
            return await Get<AppInsights>("Insights");
        }

    }
}
