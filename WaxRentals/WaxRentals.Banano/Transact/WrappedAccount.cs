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

        public string Address { get { return _account.Address; } }

        private Account _account;
        private RpcClients _rpc;

        public WrappedAccount(BananoSeed seed, uint index, RpcClients rpc)
        {
            _account = new Account(seed.Seed, index, Protocol.Prefix);
            _rpc = rpc;
        }

        public async Task Send(string target, BigDecimal banano)
        {
            await _rpc.Node.UpdateAccountAsync(_account);
            var work = await GenerateWork(_account);
            var send = Block.CreateSendBlock(
                _account,
                target,
                Amount.FromNano(BananoToNano(banano).ToString()),
                work);
            await _rpc.Node.ProcessAsync(send);
        }

        public async Task<BigDecimal> Receive()
        {
            var pending = await _rpc.Node.PendingBlocksAsync(_account.Address, int.MaxValue);

            BigDecimal result = 0;
            if (pending?.PendingBlocks != null)
            {
                foreach (var block in pending.PendingBlocks)
                {
                    try
                    {
                        var amount = BigDecimal.Parse(block.Value.Amount);
                        if (amount >= Protocol.MinimumTransaction) // Filter out blocks smaller than the minimum.
                        {
                            await _rpc.Node.UpdateAccountAsync(_account);
                            var work = await GenerateWork(_account);
                            var receive = Block.CreateReceiveBlock(_account, block.Value, work);
                            await _rpc.Node.ProcessAsync(receive);
                            result += amount;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
            return result;
        }

        private async Task<string> GenerateWork(Account account)
        {
            var work = await _rpc.WorkServer.WorkGenerateAsync(
                account.Opened ? account.Frontier : account.PublicKey.BytesToHex()
            );
            return work?.Work;
        }

        private BigDecimal BananoToNano(BigDecimal banano)
        {
            return banano * 0.1;
        }
        
    }
}
