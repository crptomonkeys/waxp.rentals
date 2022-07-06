using System;
using System.Threading.Tasks;
using WaxRentals.Service.Shared.Entities;
using WaxRentals.Service.Shared.Entities.Input;

namespace WaxRentals.Service.Shared.Connectors
{
    public interface IPurchaseService
    {
        Task<Result> Create(decimal amount, string transaction, string paymentAddress, decimal banano, Status status);
        Task<Result<PurchaseInfo>> Next();
        Task<Result> Process(int id, string transaction);
    }

    internal class PurchaseService : Connector, IPurchaseService
    {

        public PurchaseService(Uri baseUrl, ITrackService log) : base(baseUrl, log) { }

        public async Task<Result> Create(decimal amount, string transaction, string paymentAddress, decimal banano, Status status)
        {
            var input = new NewPurchaseInput { Amount = amount, Transaction = transaction, BananoPaymentAddress = paymentAddress, Banano = banano, Status = status };
            return await Post("Create", input);
        }

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
