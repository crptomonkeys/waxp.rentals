﻿using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using N2.Pow;
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
        private readonly WorkServer _workServer;
        private readonly ILog _log;

        public WrappedAccount(BananoSeed seed, uint index, RpcClients rpc, WorkServer workServer, ILog log)
        {
            _account = new Account(seed.Seed, index, Protocol.Prefix);
            _index = index;
            _rpc = rpc;
            _workServer = workServer;
            _log = log;
        }

        public string BuildLink(decimal amount)
        {
            var raw = Amount.NanoToRaw(amount * 0.1m);
            return $"banano:{Address}?amount={raw}";
        }

        #region " Send "

        public async Task<string> Send(string target, decimal banano)
        {
            if (banano > 0)
            {
                await _rpc.Node.UpdateAccountAsync(_account);
                var work = await GenerateWork();
                var send = Block.CreateSendBlock(
                    _account,
                    target,
                    Amount.FromNano((banano * 0.1m).ToString()), // Nano has one more order of magnitude from raw to nano.
                    work);
                var response = await _rpc.Node.ProcessAsync(send);
                return response.Hash;
            }
            return null;
        }

        #endregion

        #region " Receive "

        public async Task<decimal> Receive()
        {
            var blocks = await PullReceivableBlocks();
            BigDecimal result = 0;
            foreach (var block in blocks)
            {
                try
                {
                    await _rpc.Node.UpdateAccountAsync(_account);
                    var work = await GenerateWork();
                    var receive = Block.CreateReceiveBlock(_account, block, work);
                    var response = await _rpc.Node.ProcessAsync(receive);
                    
                    var amount = BigInteger.Parse(block.Amount);
                    result += amount;
                }
                catch (Exception ex)
                {
                    await _log.Error(ex, context: block);
                }
            }
            return Scale(result);
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

        public async Task<decimal> GetBalance()
        {
            if (_index > 0)
            {
                // Storage account receive happens elsewhere for tracking purposes.
                await Receive();
            }

            await _rpc.Node.UpdateAccountAsync(_account);
            if (_account.Opened)
            {
                var info = await _rpc.Node.AccountInfoAsync(
                    Address,
                    confirmed: true,
                    representative: false,
                    pending: false,
                    weight: false);
                var balance = BigDecimal.Parse(info.Balance);
                return Scale(balance);
            }
            return 0;
        }

        #endregion

        #region " Work "

        public async Task<string> GenerateWork()
        {
            await _rpc.Node.UpdateAccountAsync(_account);
            var hash = _account.Opened ? _account.Frontier : _account.PublicKey.BytesToHex();
            //var work = await _rpc.WorkServer.WorkGenerateAsync(hash, "fffffe0000000000");
            //return work?.Work;
            var response = await _workServer.GenerateWork(hash);
            return response.WorkResult?.Work;
        }

        #endregion

        #region " Scale "

        private static decimal Scale(BigDecimal value)
        {
            // Converting implicitly to decimal uses the full Mantissa rather
            // than the scaled value, so have to use string in between.
            var scaled = value * (1 / Math.Pow(10, Protocol.Decimals));
            return decimal.Parse(scaled.ToString());
        }

        #endregion

    }
}
