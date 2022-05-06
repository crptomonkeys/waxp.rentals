using System;
using System.Threading.Tasks;
using Nano.Net;
using Nano.Net.Extensions;
using Nano.Net.Numbers;
using WaxRentals.Banano.Config;
using static WaxRentals.Banano.Config.Constants;

namespace WaxRentals.Banano.Transact
{
    public class WrappedAccount : ITransact
    {

        private BananoSeed _seed;
        private uint _index;
        private RpcClient _node;
        private RpcClient _work;

        public WrappedAccount(BananoSeed seed, uint index, RpcClient node, RpcClient work)
        {
            _seed = seed;
            _index = index;
            _node = node;
            _work = work;
        }

        public async Task Send(string target, decimal banano)
        {
            var account = await BuildAccount();
            var work = await GenerateWork(account);
            var send = Block.CreateSendBlock(
                account,
                target,
                Amount.FromNano(BananoToNano(banano).ToString()),
                work);
            await _node.ProcessAsync(send);
        }

        //public async Task CheckRepresentative()
        //{
        //    var account = await BuildAccount();
        //    if (!string.Equals(account.Representative, Protocol.Representative, StringComparison.OrdinalIgnoreCase))
        //    {
        //        var work = await _work.WorkGenerateAsync(account.Frontier);
        //        var change = Block.CreateChangeBlock(account, Protocol.Representative, work.Work);
        //        await _node.ProcessAsync(change);
        //    }
        //}

        public async Task<BigDecimal> Receive()
        {
            var account = new Account(_seed.Seed, _index, Protocol.Prefix);
            var pending = await _node.PendingBlocksAsync(account.Address, int.MaxValue);

            BigDecimal result = 0;
            if (pending?.PendingBlocks != null)
            {
                foreach (var block in pending.PendingBlocks)
                {
                    try
                    {
                        await _node.UpdateAccountAsync(account);
                        var work = await GenerateWork(account);
                        var receive = Block.CreateReceiveBlock(account, block.Value, work);
                        await _node.ProcessAsync(receive);
                        result += NanoToBanano(BigDecimal.Parse(block.Value.Amount));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
            return result;
        }

        private async Task<Account> BuildAccount()
        {
            var account = new Account(_seed.Seed, _index, Protocol.Prefix);
            await _node.UpdateAccountAsync(account);
            return account;
        }

        private async Task<string> GenerateWork(Account account)
        {
            var work = await _work.WorkGenerateAsync(
                account.Opened ? account.Frontier : account.PublicKey.BytesToHex()
            );
            return work?.Work;
        }

        private decimal BananoToNano(decimal banano)
        {
            return banano / 10;
        }
        
        private BigDecimal NanoToBanano(BigDecimal nano)
        {
            return nano * 10;
        }

    }
}
