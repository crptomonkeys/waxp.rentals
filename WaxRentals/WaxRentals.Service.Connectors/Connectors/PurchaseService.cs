using System;
using System.Threading.Tasks;
using WaxRentals.Service.Shared.Entities;
using WaxRentals.Service.Shared.Entities.Input;

namespace WaxRentals.Service.Shared.Connectors
{
    public interface IPurchaseService
    {
        Task<Result<PurchaseInfo>> Next();
        Task<Result> Process(int id, string transaction);
    }

    internal class PurchaseService : Connector, IPurchaseService
    {

        public PurchaseService(Uri baseUrl, ITrackService log) : base(baseUrl, log) { }

        public async Task<Result<PurchaseInfo>> Next()
        {
            return await Get<PurchaseInfo>("Next");
        }

        public async Task<Result> Process(int id, string transaction)
        {
            var input = new ProcessInput { Id = id, Transaction = transaction };
            return await Post("Process", input);
        }

    }
}
