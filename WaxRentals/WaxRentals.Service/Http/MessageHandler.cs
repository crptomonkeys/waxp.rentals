using WaxRentals.Data.Entities;
using WaxRentals.Data.Manager;
using WaxRentals.Service.Shared.Http;

#nullable disable

namespace WaxRentals.Service.Http
{
    internal class MessageHandler : InspectingHandler
    {

        private ILog Log { get; }

        public MessageHandler(HttpMessageHandler innerHandler, ILog log)
            : base(innerHandler)
        {
            Log = log;
        }

        protected async override Task HandleRequest(string url, string fullRequest, Guid correlationId)
        {
            await Log.Message(correlationId, url, MessageDirection.Out, fullRequest);
        }

        protected async override Task HandleResponse(string url, string fullResponse, Guid correlationId)
        {
            await Log.Message(correlationId, url, MessageDirection.In, fullResponse);
        }

    }
}
