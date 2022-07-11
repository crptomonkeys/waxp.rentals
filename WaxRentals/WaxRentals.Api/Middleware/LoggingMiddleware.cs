using System.Net;
using System.Net.Http.Headers;
using WaxRentals.Service.Shared.Connectors;
using WaxRentals.Service.Shared.Entities.Input;

// https://stackoverflow.com/a/67510839/128217

namespace WaxRentals.Api.Middleware
{
    public class LoggingMiddleware
    {

        private RequestDelegate Next { get; }
        private ITrackService Track { get; }

        public LoggingMiddleware(RequestDelegate next, ITrackService track)
        {
            Next = next;
            Track = track;
        }
        
        public async Task Invoke(HttpContext context)
        {
            var request = context.Request;
            var url = request.Path;
            var correlationId = Guid.NewGuid();

            using var copyStream = new MemoryStream();
            await request.Body.CopyToAsync(copyStream);
            copyStream.Seek(0, SeekOrigin.Begin);
            using var copyReader = new StreamReader(copyStream);

            var fullRequest = GetFullMessage(
                await copyReader.ReadToEndAsync(),
                $"{request.Method} {request.Path}",
                request.Headers
            );
            await Log(MessageLogDirection.In, url, fullRequest, correlationId);

            copyStream.Seek(0, SeekOrigin.Begin);
            request.Body = copyStream;

            var response = context.Response;
            using (var swapStream = new MemoryStream())
            {
                var originalBody = response.Body;
                response.Body = swapStream;
                await Next(context);
                swapStream.Seek(0, SeekOrigin.Begin);

                using (var reader = new StreamReader(response.Body))
                {
                    var fullResponse = GetFullMessage(
                        await reader.ReadToEndAsync(),
                        $"{response.StatusCode} {(HttpStatusCode)response.StatusCode}",
                        response.Headers
                    );
                    await Log(MessageLogDirection.Out, url, fullResponse, correlationId);

                    swapStream.Seek(0, SeekOrigin.Begin);
                    await swapStream.CopyToAsync(originalBody);
                    response.Body = originalBody;
                }
            }
        }

        private async Task Log(MessageLogDirection direction, string url, string message, Guid correlationId)
        {
            await Track.Message(
                new MessageLog
                {
                    Direction = direction,
                    Url = url,
                    Message = message,
                    RequestId = correlationId
                }
            );
        }

        private static string GetFullMessage(string body, string lead, IHeaderDictionary headers)
        {
            // For some reason, we need to force the Content-Length header for it to be included.
            headers.OfType<HttpContentHeaders>().Select(header => header.ContentLength);

            var combined = string.Join(
                Environment.NewLine,
                headers.Select(header => $"{header.Key}: {string.Join(", ", header.Value)}")
            );

            return string.Join(
                $"{Environment.NewLine}{Environment.NewLine}",
                lead,
                combined,
                body
            );
        }

    }

    public static class LoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseMessageLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoggingMiddleware>();
        }
    }
}
