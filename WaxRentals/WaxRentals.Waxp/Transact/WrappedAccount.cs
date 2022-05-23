using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eos.Cryptography;
using Eos.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WaxRentals.Data.Manager;
using static WaxRentals.Waxp.Config.Constants;

namespace WaxRentals.Waxp.Transact
{
    internal class WrappedAccount : IWaxAccount
    {

        public string Account { get; }

        private readonly PrivateKey _active;
        private readonly IClientFactory _client;
        private readonly IDataFactory _factory;
        private readonly List<Authorization> _authorization;

        public WrappedAccount(string account, PrivateKey active, IClientFactory client, IDataFactory factory)
        {
            Account = account;
            _active = active;
            _client = client;
            _factory = factory;

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

        #region " Balances "

        public async Task<AccountBalances> GetBalances()
        {
            var balances = new AccountBalances();
            var apiTask = _client.ProcessApi(async client =>
            {
                var account = await client.GetAccount(Account);
                if (account != null)
                {
                    decimal.TryParse(account.CoreLiquidBalance?.Split(' ')?.FirstOrDefault(), out decimal liquid);
                    decimal.TryParse(account.RefundRequest?.CpuAmount?.Split(' ')?.FirstOrDefault(), out decimal cpuRefund);
                    decimal.TryParse(account.RefundRequest?.NetAmount?.Split(' ')?.FirstOrDefault(), out decimal netRefund);
                    balances.Available = liquid;
                    balances.Unstaking = cpuRefund + netRefund;
                }
            });
            var jsonTask = GetValueFromJson(
                string.Format(Locations.StakedEndpointFormat, Account),
                Protocol.StakedWaxFormats.Select(format => string.Format(format, Account)),
                Protocol.Decimals
            );

            await Task.WhenAll(apiTask, jsonTask);
            balances.Staked = await jsonTask;
            return balances;
        }

        private async Task<decimal> GetValueFromJson(string url, IEnumerable<string> selectors, int scale)
        {
            try
            {
                var json = JObject.Parse(await new QuickTimeoutWebClient().DownloadStringTaskAsync(url, TimeSpan.FromSeconds(5)));
                double result = 0;
                foreach (var selector in selectors)
                {
                    var nodes = json.SelectTokens(selector);
                    foreach (var node in nodes)
                    {
                        result += node.Value<double>() / Math.Pow(10, scale);
                    }
                }
                return Convert.ToDecimal(result);
            }
            catch (Exception ex)
            {
                await _factory.Log.Error(ex);
                return 0;
            }
        }

        #endregion

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
                    Account = new Name("eosio.token"),
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

        private class RefundAction : Eos.Models.Action<RefundData>
        {
            public override Name Account => "eosio";
            public override Name Name => "refund";
        }

        #endregion

    }
}
