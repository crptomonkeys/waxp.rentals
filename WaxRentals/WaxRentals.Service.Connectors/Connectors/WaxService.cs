using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WaxRentals.Service.Shared.Entities;
using WaxRentals.Service.Shared.Entities.Input;

#nullable disable

namespace WaxRentals.Service.Shared.Connectors
{
    public interface IWaxService
    {
        Task<Result<IEnumerable<Nft>>> Nfts();
        Task<Result<IEnumerable<WaxTransferInfo>>> LatestTransfers();
        Task<Result> Sweep();
        Task<Result<NewStakeInfo>> Stake(int cpu, int net, string target, string source = null, int? days = null);
        Task<Result<string>> Unstake(int cpu, int net, string target, string source);
        Task<Result<string>> Send(string recipient, decimal amount, string memo = null, string source = null);
        Task<Result<string>> SendAsset(string recipient, string assetId, string memo = null);
    }

    internal class WaxService : Connector, IWaxService
    {

        public WaxService(Uri baseUrl, ITrackService log) : base(baseUrl, log) { }

        public async Task<Result<IEnumerable<Nft>>> Nfts()
        {
            return await Get<IEnumerable<Nft>>("Nfts");
        }

        public async Task<Result<IEnumerable<WaxTransferInfo>>> LatestTransfers()
        {
            return await Get<IEnumerable<WaxTransferInfo>>("LatestTransfers");
        }

        public async Task<Result> Sweep()
        {
            return await Post("Sweep");
        }

        public async Task<Result<NewStakeInfo>> Stake(int cpu, int net, string target, string source = null, int? days = null)
        {
            var input = new StakeInput { Cpu = cpu, Net = net, Target = target, Source = source, Days = days };
            return await Post<NewStakeInfo>("Stake", input);
        }

        public async Task<Result<string>> Unstake(int cpu, int net, string target, string source)
        {
            var input = new StakeInput { Cpu = cpu, Net = net, Target = target, Source = source };
            return await Post<string>("Unstake", input);
        }

        public async Task<Result<string>> Send(string recipient, decimal amount, string memo = null, string source = null)
        {
            var input = new SendWaxInput { Recipient = recipient, Amount = amount, Memo = memo, Source = source };
            return await Post<string>("Send", input);
        }

        public async Task<Result<string>> SendAsset(string recipient, string assetId, string memo = null)
        {
            var input = new SendNftInput { Recipient = recipient, AssetId = assetId, Memo = memo };
            return await Post<string>("SendAsset", input);
        }

    }
}
