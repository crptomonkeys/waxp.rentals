using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

#nullable disable

namespace WaxRentals.Service.Shared.Http
{
    public abstract class InspectingHandler : DelegatingHandler
    {

        public InspectingHandler(HttpMessageHandler innerHandler) : base(innerHandler) { }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var url = request.RequestUri?.OriginalString;
            var correlationId = Guid.NewGuid();

            var fullRequest = await GetFullMessage(
                request.GetBody(),
                $"{request.Method} {request.RequestUri?.OriginalString}",
                request.Headers,
                request.Content?.Headers
            );
            await HandleRequest(url, fullRequest, correlationId);

            var response = await base.SendAsync(request, cancellationToken);
            var fullResponse = await GetFullMessage(
                response.GetBody(),
                $"{(int)response.StatusCode} {response.StatusCode}",
                response.Headers,
                response.Content.Headers
            );
            await HandleResponse(url, fullResponse, correlationId);

            return response;
        }

        protected abstract Task HandleRequest(string url, string fullRequest, Guid correlationId);
        protected abstract Task HandleResponse(string url, string fullResponse, Guid correlationId);

        private static async Task<string> GetFullMessage(Task<string> body, string lead, params HttpHeaders[] headers)
        {
            // For some reason, we need to force the Content-Length header for it to be included.
            headers.OfType<HttpContentHeaders>().Select(header => header.ContentLength);

            var flat = headers.Where(h => h != null).SelectMany(header => header);
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
}
