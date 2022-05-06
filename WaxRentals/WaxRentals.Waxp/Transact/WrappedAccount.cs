using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Eos;
using Eos.Api;
using Eos.Cryptography;
using Eos.Models;
using Newtonsoft.Json;

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

        public async Task<string> Stake(string account, decimal cpu, decimal net)
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
            var info = await _transact.GetInfo();
            var signed = await _transact.SignTransaction(transaction, _active);
            //return await _transact.PushTransaction(signed);
            return await PushTransaction(signed);
        }

        public readonly int ApiVersion = 1;

        public const string ChainGroup = "chain";

        public const string HistoryGroup = "history";

        private HttpClient httpClient = new HttpClient() { BaseAddress = new Uri("https://api-wax.eosauthority.com") };



        private class LongCurrency : Currency
        {
            public LongCurrency(string value) : base(value) { }
            public override string ToString()
            {
                return $"{Amount:0.00000000} {Ticker}";
            }
        }

        internal class PushTransactionRequest
        {
            [JsonProperty("signatures")]
            public List<string> Signatures { get; set; }

            [JsonProperty("compression")]
            public string Compression { get; set; }

            [JsonProperty("packed_trx")]
            public string PackedTransaction { get; set; }
        }
        internal class PushTransactionResponse
        {
            [JsonProperty("transaction_id")]
            public string TransactionId { get; set; }

            [JsonProperty("processed")]
            public object Processed { get; set; }
        }
        public async Task<string> PushTransaction(SignedTransaction transaction)
        {
            PushTransactionRequest request = new PushTransactionRequest()
            {
                PackedTransaction = transaction.PackedTransaction,
                Signatures = transaction.Signatures
            };

            var response = await PostAsync<PushTransactionResponse>(request, $"/v{ApiVersion}/{ChainGroup}/push_transaction");

            string result = response.TransactionId;

            return result;
        }

        protected async Task<T> PostAsync<T>(object request, string uri)
        {
#if DEBUG
            string requestCmd = JsonConvert.SerializeObject(request, Formatting.Indented);
#else
            string requestCmd = JsonConvert.SerializeObject(request);
#endif
            string response;

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            using (HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Post, uri))
            {
                req.Content = new StringContent(requestCmd, Encoding.UTF8, "application/json");
                response = await (await httpClient.SendAsync(req)).Content.ReadAsStringAsync();
            }

            if (response.Contains("{\"code\":") && response.Contains(",\"message\":") && response.Contains(",\"error\":"))
            {
                var error = JsonConvert.DeserializeObject<ApiException>(response);
                throw error;
            }

            T res = JsonConvert.DeserializeObject<T>(response);
            if (res != null) return res;

            return default(T);
        }


    }
}
