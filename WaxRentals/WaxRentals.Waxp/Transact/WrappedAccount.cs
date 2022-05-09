using System.Collections.Generic;
using System.Threading.Tasks;
using Eos.Cryptography;
using Eos.Models;

namespace WaxRentals.Waxp.Transact
{
    internal class WrappedAccount : ITransact
    {

        private readonly string _account;
        private readonly PrivateKey _active;
        private readonly ClientFactory _client;

        public WrappedAccount(string account, PrivateKey active, ClientFactory client)
        {
            _account = account;
            _active = active;
            _client = client;
        }

        public async Task<bool> Stake(string account, decimal cpu, decimal net)
        {
            Transaction transaction = null;
            var success = await _client.ProcessApi(
                async client => transaction = await client.CreateTransaction(
                    new List<IAction>
                    {
                        new StakeAction
                        {
                            Authorization = new List<Authorization>
                            {
                                new Authorization
                                {
                                    Actor = new Name(_account),
                                    Permission = new Name("active")
                                }
                            },
                            Data = new StakeData
                            {
                                Cpu = new LongCurrency($"{cpu} WAX"),
                                Net = new LongCurrency($"{net} WAX"),
                                From = new Name(_account),
                                Receiver = new Name(account),
                                Transfer = false
                            }
                        }
                    }
                )
            );
            return success && await Process(transaction);
        }

        public async Task<bool> Unstake(string account, decimal cpu, decimal net)
        {
            Transaction transaction = null;
            var success = await _client.ProcessApi(
                async client => transaction = await client.CreateTransaction
                (
                    new List<IAction>
                    {
                        new UnstakeAction
                        {
                            Authorization = new List<Authorization>
                            {
                                new Authorization
                                {
                                    Actor = new Name(_account),
                                    Permission = new Name("active")
                                }
                            },
                            Data = new UnstakeData
                            {
                                Cpu = new LongCurrency($"{cpu} WAX"),
                                Net = new LongCurrency($"{net} WAX"),
                                From = new Name(_account),
                                Receiver = new Name(account),
                                Transfer = false
                            }
                        }
                    }
                )
            );
            return success && await Process(transaction);
        }

        private async Task<bool> Process(Transaction transaction)
        {
            return await _client.ProcessApi(
                async client =>
                {
                    var signed = await client.SignTransaction(transaction, _active);
                    await client.PushTransaction(signed);
                }
            );
        }

        private class LongCurrency : Currency
        {
            public LongCurrency(string value) : base(value) { }
            public override string ToString()
            {
                return $"{Amount:0.00000000} {Ticker}";
            }
        }
        
    }
}
