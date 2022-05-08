using System.Collections.Generic;
using System.Threading.Tasks;
using Eos.Api;
using Eos.Cryptography;
using Eos.Models;

namespace WaxRentals.Waxp.Transact
{
    public class WrappedAccount
    {

        private readonly string _account;
        private readonly PrivateKey _active;
        private readonly NodeApiClient _history;
        private readonly NodeApiClient _transact;

        public WrappedAccount(string account, PrivateKey active, NodeApiClient history, NodeApiClient transact)
        {
            _account = account;
            _active = active;
            _history = history;
            _transact = transact;
        }

        public async Task<bool> Stake(string account, decimal cpu, decimal net)
        {
            var transaction = await _transact.CreateTransaction
            (
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
            );
            return await Process(transaction);
        }

        public async Task<bool> Unstake(string account, decimal cpu, decimal net)
        {
            var transaction = await _transact.CreateTransaction
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
            );
            return await Process(transaction);
        }

        private async Task<bool> Process(Transaction transaction)
        {
            // Catch and log exceptions!
            var signed = await _transact.SignTransaction(transaction, _active);
            await _transact.PushTransaction(signed);
            return true;
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
