using WaxRentals.Data.Entities;
using WaxRentals.Data.Manager;
using WaxRentals.Service.Shared.Http;

#nullable disable

namespace WaxRentals.Service.Http
{
    internal class MessageHandler : InspectingHandler
    {

        private IDataFactory Factory { get; }

        public MessageHandler(HttpMessageHandler innerHandler, IDataFactory factory)
            : base(innerHandler)
        {
            Factory = factory;
        }

        protected async override Task HandleRequest(string url, string fullRequest, Guid correlationId)
        {
            await Factory.Log.Message(correlationId, url, MessageDirection.Out, fullRequest);
        }

        protected async override Task HandleResponse(string url, string fullResponse, Guid correlationId)
        {
            await Factory.Log.Message(correlationId, url, MessageDirection.In, fullResponse);
        }

    }
}
