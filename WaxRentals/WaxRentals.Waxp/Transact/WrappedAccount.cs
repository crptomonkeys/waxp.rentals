using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eos.Cryptography;
using Eos.Models;
using Newtonsoft.Json;

namespace WaxRentals.Waxp.Transact
{
    internal class WrappedAccount : IWaxAccount
    {

        public string Account { get; }

        private readonly PrivateKey _active;
        private readonly ClientFactory _client;
        private readonly List<Authorization> _authorization;

        public WrappedAccount(string account, PrivateKey active, ClientFactory client)
        {
            Account = account;
            _active = active;
            _client = client;

            _authorization = new List<Authorization>
            {
                new Authorization
                {
                    Actor = new Name(Account),
                    Permission = new Name("active")
                }
            };
        }

        #region " IWaxAccount "

        public async Task<AccountBalances> GetBalances()
        {
            var balances = new AccountBalances();
            var apiTask = _client.ProcessApi(async client =>
            {
                var account = await client.GetAccount(Account);
                decimal.TryParse(account.CoreLiquidBalance?.Split(' ')?.FirstOrDefault(), out decimal liquid);
                decimal.TryParse(account.RefundRequest?.CpuAmount?.Split(' ')?.FirstOrDefault(), out decimal cpuRefund);
                decimal.TryParse(account.RefundRequest?.NetAmount?.Split(' ')?.FirstOrDefault(), out decimal netRefund);
                balances.Available = liquid;
                balances.Unstaking = cpuRefund + netRefund;
            });

            await Task.WhenAll(apiTask);
            return balances;
        }

        public async Task<(bool, string)> Stake(string account, decimal cpu, decimal net)
        {
            return await Process(
                new StakeAction
                {
                    Authorization = _authorization,
                    Data = new StakeData
                    {
                        Cpu = new LongCurrency($"{cpu} WAX"),
                        Net = new LongCurrency($"{net} WAX"),
                        From = new Name(Account),
                        Receiver = new Name(account),
                        Transfer = false
                    }
                }
            );
        }

        public async Task<(bool, string)> Unstake(string account, decimal cpu, decimal net)
        {
            return await Process(
                new UnstakeAction
                {
                    Authorization = _authorization,
                    Data = new UnstakeData
                    {
                        Cpu = new LongCurrency($"{cpu} WAX"),
                        Net = new LongCurrency($"{net} WAX"),
                        From = new Name(Account),
                        Receiver = new Name(account),
                        Transfer = false
                    }
                }
            );
        }

        public async Task<(bool, string)> ClaimRefund()
        {
            return await Process(
                new RefundAction
                {
                    Authorization = _authorization,
                    Data = new RefundData
                    {
                        Owner = new Name(Account)
                    }
                }
            );
        }

        public async Task<(bool, string)> Send(string account, decimal wax)
        {
            return await Process(
                new TransferAction
                {
                    Authorization = _authorization,
                    Data = new TransferData
                    {
                        From = new Name(Account),
                        To = new Name(account),
                        Quantity = new LongCurrency($"{wax} WAX")
                    }
                }
            );
        }

        #endregion

        #region " Not Used "

        private async Task<(bool, string)> BuyRam(string account, int bytes)
        {
            return await Process(
                new BuyRamAction
                {
                    Authorization = _authorization,
                    Data = new BuyRamData
                    {
                        Bytes = bytes,
                        Payer = new Name(Account),
                        Receiver = new Name(account)
                    }
                }
            );
        }

        private async Task<(bool, string)> CreateAccount(string account, int ram, decimal cpu, decimal net)
        {
            return await Process(
                new NewAccountAction
                {
                    Authorization = _authorization,
                    Data = new NewAccountData
                    {
                        Creator = new Name(Account),
                        Name = new Name(account),
                        Owner = new Authority
                        {
                            Keys = new List<AuthorityKey>
                            {
                                new AuthorityKey { Key = new PublicKey("EOS8U61Z9FkfdY4xGxZNLijnKCMtN4TLxdUQ2vLzdv95r2KofQo2j"), Weight = 1 }
                            },
                            Threshold = 1
                        },
                        Active = new Authority
                        {
                            Keys = new List<AuthorityKey>
                            {
                                new AuthorityKey { Key = new PublicKey("EOS8mXEzdgHHdbmt4qFvFJv7T1ZHFQCSNVcQC7BwHJMK5TK6qCWqz"), Weight = 1 }
                            },
                            Threshold = 1
                        }
                    }
                },
                new BuyRamAction
                {
                    Authorization = _authorization,
                    Data = new BuyRamData
                    {
                        Bytes = ram,
                        Payer = new Name(Account),
                        Receiver = new Name(account)
                    }
                },
                 new StakeAction
                 {
                     Authorization = _authorization,
                     Data = new StakeData
                     {
                         Cpu = new LongCurrency($"{cpu} WAX"),
                         Net = new LongCurrency($"{net} WAX"),
                         From = new Name(Account),
                         Receiver = new Name(account),
                         Transfer = true
                     }
                 }
            );
        }

        #endregion

        #region " Process "

        private async Task<(bool, string)> Process(params IAction[] actions)
        {
            Transaction transaction = null;
            var success = await _client.ProcessApi(async client =>
                transaction = await client.CreateTransaction(actions.ToList())
            );
            if (success)
            {
                return await Process(transaction);
            }
            else
            {
                return (false, null);
            }
        }

        private async Task<(bool, string)> Process(Transaction transaction)
        {
            string hash = default;
            var success = await _client.ProcessApi(
                async client =>
                {
                    var signed = await client.SignTransaction(transaction, _active);
                    hash = await client.PushTransaction(signed);
                }
            );
            return (success, hash);
        }

        #endregion

        #region " Derived Classes "

        private class LongCurrency : Currency
        {
            public LongCurrency(string value) : base(value) { }
            public override string ToString()
            {
                return $"{Amount:0.00000000} {Ticker}";
            }
        }

        private class RefundData
        {
            [JsonProperty("owner")]
            public Name Owner { get; set; }
        }

        private class RefundAction : Action<RefundData>
        {
            public override Name Account => "eosio";
            public override Name Name => "refund";
        }

        #endregion

    }
}
