using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nano.Net;
using Nano.Net.Extensions;
using Nano.Net.Numbers;
using WaxRentals.Banano.Config;
using WaxRentals.Data.Manager;
using static WaxRentals.Banano.Config.Constants;

namespace WaxRentals.Banano.Transact
{
    internal class WrappedAccount : ITransact
    {

        public string Address { get { return _account.Address; } }

        private Account _account;
        private RpcClients _rpc;
        private ILog _log;

        public WrappedAccount(BananoSeed seed, uint index, RpcClients rpc, ILog log)
        {
            _account = new Account(seed.Seed, index, Protocol.Prefix);
            _rpc = rpc;
            _log = log;
        }

        #region " Send "

        public async Task<string> Send(string target, BigDecimal banano)
        {
            await _rpc.Node.UpdateAccountAsync(_account);
            var work = await GenerateWork(_account);
            var send = Block.CreateSendBlock(
                _account,
                target,
                Amount.FromNano(BananoToNano(banano).ToString()),
                work);
            var response = await _rpc.Node.ProcessAsync(send);
            return response.Hash;
        }

        private BigDecimal BananoToNano(BigDecimal banano)
        {
            return banano * 0.1;
        }

        #endregion

        #region " Receive "

        public async Task<bool> HasPendingBlocks()
        {
            var blocks = await PullReceivableBlocks();
            return blocks.Any();
        }

        public async Task<IEnumerable<ReceivableBlock>> PullReceivableBlocks()
        {
            var result = new List<ReceivableBlock>();

            var pending = await _rpc.Node.PendingBlocksAsync(_account.Address, int.MaxValue);
            if (pending?.PendingBlocks != null)
            {
                foreach (var block in pending.PendingBlocks)
                {
                    try
                    {
                        var amount = BigDecimal.Parse(block.Value.Amount);
                        if (amount >= Protocol.MinimumTransaction) // Filter out blocks smaller than the minimum.
                        {
                            result.Add(block.Value);
                        }
                    }
                    catch (Exception ex)
                    {
                        await _log.Error(ex, block);
                    }
                }
            }

            return result;
        }

        public async Task<BigDecimal> Receive()
        {
            var blocks = await PullReceivableBlocks();
            BigDecimal result = 0;
            foreach (var block in blocks)
            {
                try
                {
                    await _rpc.Node.UpdateAccountAsync(_account);
                    var work = await GenerateWork(_account);
                    var receive = Block.CreateReceiveBlock(_account, block, work);
                    await _rpc.Node.ProcessAsync(receive);
                    result += BigDecimal.Parse(block.Amount);
                }
                catch (Exception ex)
                {
                    await _log.Error(ex, block);
                }
            }
            return result;
        }

        #endregion

        #region " Work "

        private async Task<string> GenerateWork(Account account)
        {
            var work = await _rpc.WorkServer.WorkGenerateAsync(
                account.Opened ? account.Frontier : account.PublicKey.BytesToHex()
            );
            return work?.Work;
        }

        #endregion

    }
}
