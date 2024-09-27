using System;
using System.Threading.Tasks;
using WaxRentals.Service.Shared.Entities;
using WaxRentals.Service.Shared.Entities.Input;

#nullable disable

namespace WaxRentals.Service.Shared.Connectors
{
    public interface IBananoService
    {
        Task<Result<decimal>> RentalAccountBalance(int id);
        Task<Result<decimal>> WelcomeAccountBalance(int id);
        Task<Result<string>> SweepRentalAccount(int id);
        Task<Result<string>> SweepWelcomeAccount(int id);
        Task<Result<decimal>> CompleteSweeps();
        Task<Result<string>> Send(string address, decimal amount, string reason = null);
    }

    internal class BananoService : Connector, IBananoService
    {

        public BananoService(Uri baseUrl, ITrackService log) : base(baseUrl, log) { }

        public async Task<Result<decimal>> RentalAccountBalance(int id)
        {
            return await Get<decimal>($"RentalAccountBalance/{id}");
        }

        public async Task<Result<decimal>> WelcomeAccountBalance(int id)
        {
            return await Get<decimal>($"WelcomeAccountBalance/{id}");
        }

        public async Task<Result<string>> SweepRentalAccount(int id)
        {
            return await Post<string>("SweepRentalAccount", id);
        }

        public async Task<Result<string>> SweepWelcomeAccount(int id)
        {
            return await Post<string>("SweepWelcomeAccount", id);
        }

        public async Task<Result<decimal>> CompleteSweeps()
        {
            return await Post<decimal>("CompleteSweeps");
        }

        public async Task<Result<string>> Send(string address, decimal amount, string reason = null)
        {
            var input = new SendBananoInput { Recipient = address, Amount = amount, Reason = reason };
            return await Post<string>("Send", input);
        }

    }
}
