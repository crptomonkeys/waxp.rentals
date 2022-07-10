using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WaxRentals.Api.Entities;
using WaxRentals.Service.Shared.Connectors;
using WaxRentalsWeb.Config;

namespace WaxRentalsWeb.Net
{
    public class ApiProxy
    {

        public ApiContext Endpoints { get; }

        protected HttpClient Client { get; }
        protected ITrackService Log { get; }
        protected static JsonSerializerOptions SerializerOptions { get; }

        static ApiProxy()
        {
            SerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        }

        public ApiProxy(ApiContext endpoints, ITrackService log = null)
        {
            Endpoints = endpoints;
            Client = new();
            Log = log;
        }

        public async Task<Result<TOut>> Post<TOut>(string path)
        {
            return await Process<TOut>(async () =>
                await Client.PostAsync(path, null)
            );
        }

        public async Task<Result<TOut>> Post<TOut>(string path, object data)
        {
            var json = JsonSerializer.Serialize(data);
            return await Process<TOut>(async () =>
                await Client.PostAsync(path, new StringContent(json, Encoding.UTF8, "application/json"))
            );
        }

        public async Task<Result<TOut>> Get<TOut>(string path)
        {
            return await Process<TOut>(async () => await Client.GetAsync(path));
        }

        public async Task<Result<TOut>> Get<TOut>(string path, string id)
        {
            return await Get<TOut>($"{path}/{id}");
        }

        private async Task<Result<TOut>> Process<TOut>(Func<Task<HttpResponseMessage>> target)
        {
            try
            {
                var response = await target();
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<Result<TOut>>(content, SerializerOptions) ?? Result<TOut>.Fail($"Unable to deserialize response from server:{Environment.NewLine}{content}");
                }
                return Result<TOut>.Fail($"Unsuccessful response from server: {(int)response.StatusCode} {response.StatusCode}");
            }
            catch (Exception ex)
            {
                try
                {
                    if (Log != null)
                    {
                        await Log.Error(ex);
                    }
                    return Result<TOut>.Fail(ex.Message);
                }
                catch
                {
                    return Result<TOut>.Fail("Unknown error.");
                }
            }
        }

    }
}
