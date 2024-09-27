using System;
using System.Net.Http;
using System.Threading.Tasks;
using WaxRentals.Service.Shared.Connectors;
using WaxRentals.Service.Shared.Entities.Input;

namespace WaxRentals.Service.Shared.Http
{
    public class LoggingHandler : InspectingHandler
    {

        private ITrackService Track { get; }

        public LoggingHandler(HttpMessageHandler innerHandler, ITrackService track)
            : base(innerHandler)
        {
            Track = track;
        }

        protected async override Task HandleRequest(string url, string fullRequest, Guid correlationId)
        {
            await Log(MessageLogDirection.Out, url, fullRequest, correlationId);
        }

        protected async override Task HandleResponse(string url, string fullResponse, Guid correlationId)
        {
            await Log(MessageLogDirection.In, url, fullResponse, correlationId);
        }

        private async Task Log(MessageLogDirection direction, string url, string message, Guid correlationId)
        {
            if (Track != null)
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
        }

    }
}
