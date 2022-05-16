using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Nano.Net;
using Nano.Net.Extensions;
using Nano.Net.Numbers;
using WaxRentals.Banano.Config;
using WaxRentals.Data.Manager;
using static WaxRentals.Banano.Config.Constants;

namespace WaxRentals.Banano.Transact
{
    internal class WrappedAccount : IBananoAccount
    {

        public string Address { get { return _account.Address; } }

        private readonly Account _account;
        private readonly uint _index;
        private readonly RpcClients _rpc;
        private readonly IInsert _data;
        private readonly ILog _log;

        public WrappedAccount(BananoSeed seed, uint index, RpcClients rpc, IInsert data, ILog log)
        {
            _account = new Account(seed.Seed, index, Protocol.Prefix);
            _index = index;
            _rpc = rpc;
            _data = data;
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

        #endregion

        #region " Receive "

        public async Task<BigDecimal> Receive(bool verifyOnly)
        {
            var blocks = await PullReceivableBlocks();
            BigDecimal result = 0;
            foreach (var block in blocks)
            {
                try
                {
                    if (!verifyOnly)
                    {
                        await _rpc.Node.UpdateAccountAsync(_account);
                        var work = await GenerateWork(_account);
                        var receive = Block.CreateReceiveBlock(_account, block, work);
                        var response = await _rpc.Node.ProcessAsync(receive);
                    }
                    
                    var amount = BigInteger.Parse(block.Amount);
                    result += amount;
                }
                catch (Exception ex)
                {
                    await _log.Error(ex, context: block);
                }
            }

            if (!verifyOnly && _index > 0 && result > 0)
            {
                await Send(Protocol.Address, result / Math.Pow(10, Protocol.Decimals));
            }

            return result;
        }

        private async Task<IEnumerable<ReceivableBlock>> PullReceivableBlocks()
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
                        await _log.Error(ex, context: block);
                    }
                }
            }

            return result;
        }

        #endregion

        #region " Balance "

        public async Task<BigDecimal> GetBalance()
        {
            var info = await _rpc.Node.AccountInfoAsync(
                Address,
                confirmed: true,
                representative: false,
                pending: false,
                weight: false);
            return BigDecimal.Parse(info.Balance);
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

        private BigDecimal BananoToNano(BigDecimal banano)
        {
            return banano * 0.1;
        }

        private decimal RawToDecimal(BigInteger raw)
        {
            var banano = BananoToNano(Amount.RawToNano(raw));
            return (decimal)banano;
        }

    }
}
