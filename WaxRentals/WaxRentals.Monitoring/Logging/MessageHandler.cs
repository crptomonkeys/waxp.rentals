using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using WaxRentals.Data.Entities;
using WaxRentals.Data.Manager;

namespace WaxRentals.Monitoring.Logging
{
    public class MessageHandler : DelegatingHandler
    {

        private readonly ILog _log;

        public MessageHandler(HttpMessageHandler innerHandler, ILog log)
            : base(innerHandler)
        {
            _log = log;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var url = request.RequestUri.OriginalString;
            var requestId = Guid.NewGuid();

            var fullRequest = await GetFullMessage(
                request.GetBody(),
                $"{request.Method} {request.RequestUri.OriginalString}",
                request.Headers,
                request.Content?.Headers
            );
            await _log.Message(requestId, url, MessageDirection.Out, fullRequest);

            var response = await base.SendAsync(request, cancellationToken);
            var fullResponse = await GetFullMessage(
                response.GetBody(),
                $"{(int)response.StatusCode} {response.StatusCode}",
                response.Headers,
                response.Content.Headers
            );
            await _log.Message(requestId, url, MessageDirection.In, fullResponse);

            return response;
        }

        private static async Task<string> GetFullMessage(Task<string> body, string lead, params HttpHeaders[] headers)
        {
            // For some reason, we need to force the Content-Length header for it to be included.
            headers.OfType<HttpContentHeaders>().Select(header => header.ContentLength).ToList();

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

    internal static class HttpRequestExtensions
    {

        public static async Task<string> GetBody(this HttpRequestMessage @this)
        {
            // An empty body will show as null Content.
            if (@this.Content == null)
            {
                return null;
            }
            return await @this.Content.ReadAsStringAsync();
        }

        public static async Task<string> GetBody(this HttpResponseMessage @this)
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
