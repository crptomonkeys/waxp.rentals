using System;
using System.Net.Http;
using System.Threading.Tasks;
using Eos.Api;

namespace WaxRentals.Waxp.Transact
{
    public interface IClientFactory
    {

        Task<bool> ProcessApi(Func<NodeApiClient, Task> action);
        Task<bool> ProcessHistory(Func<HttpClient, Task> action);

    }
}
