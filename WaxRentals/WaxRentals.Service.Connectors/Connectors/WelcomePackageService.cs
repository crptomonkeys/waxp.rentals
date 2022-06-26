using System;
using System.Threading.Tasks;
using WaxRentals.Data.Manager;
using WaxRentals.Service.Shared.Entities;

namespace WaxRentals.Service.Shared.Connectors
{
    public interface IWelcomePackageService
    {
        Task<Result<NewWelcomePackage>> New(string memo);
    }

    internal class WelcomePackageService : Connector, IWelcomePackageService
    {

        public WelcomePackageService(Uri baseUrl, ILog log) : base(baseUrl, log) { }

        public async Task<Result<NewWelcomePackage>> New(string memo)
        {
            return await Post<NewWelcomePackage>("New", memo);
        }

    }
}
