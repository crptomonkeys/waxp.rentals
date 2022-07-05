using System.Net.Http;
using System.Threading.Tasks;

namespace WaxRentals.Service.Shared.Http
{
    public static class HttpRequestExtensions
    {

        public static async Task<string?> GetBody(this HttpRequestMessage @this)
        {
            // An empty body will show as null Content.
            if (@this.Content == null)
            {
                return null;
            }
            return await @this.Content.ReadAsStringAsync();
        }

        public static async Task<string?> GetBody(this HttpResponseMessage @this)
        {
            // An empty body will show as null Content.
            if (@this.Content == null)
            {
                return null;
            }
            return await @this.Content.ReadAsStringAsync();
        }

    }
}
