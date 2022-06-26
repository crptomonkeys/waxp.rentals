using System.Net.Http.Headers;
using WaxRentals.Data.Entities;
using WaxRentals.Data.Manager;

namespace WaxRentals.Service.Http
{
    internal class MessageHandler : DelegatingHandler
    {

        private IDataFactory Factory;

        public MessageHandler(HttpMessageHandler innerHandler, IDataFactory factory)
            : base(innerHandler)
        {
            Factory = factory;
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var url = request.RequestUri?.OriginalString;
            var requestId = Guid.NewGuid();

            var fullRequest = await GetFullMessage(
                request.GetBody(),
                $"{request.Method} {request.RequestUri?.OriginalString}",
                request.Headers,
                request.Content?.Headers
            );
            await Factory.Log.Message(requestId, url, MessageDirection.Out, fullRequest);

            var response = await base.SendAsync(request, cancellationToken);
            var fullResponse = await GetFullMessage(
                response.GetBody(),
                $"{(int)response.StatusCode} {response.StatusCode}",
                response.Headers,
                response.Content.Headers
            );
            await Factory.Log.Message(requestId, url, MessageDirection.In, fullResponse);

            return response;
        }

        private static async Task<string> GetFullMessage(Task<string?> body, string lead, params HttpHeaders?[] headers)
        {
            // For some reason, we need to force the Content-Length header for it to be included.
            headers.OfType<HttpContentHeaders>().Select(header => header.ContentLength).ToList();

            var flat = headers.Where(h => h != null).SelectMany(header => header); // The warning says this could be null,
                                                                                   // but it obviously can't be.
            var combined = string.Join(
                Environment.NewLine,
                flat.Select(header => $"{header.Key}: {string.Join(", ", header.Value)}")
            );

            return string.Join(
                $"{Environment.NewLine}{Environment.NewLine}",
                lead,
                combined,
                await body
            );
        }

    }

    internal static class HttpRequestExtensions
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
