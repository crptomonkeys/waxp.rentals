using Microsoft.AspNetCore.Mvc;
using WaxRentals.Data.Entities;
using WaxRentals.Data.Manager;
using WaxRentals.Monitoring.Notifications;
using WaxRentals.Service.Shared.Entities.Input;
using WaxRentals.Service.Tracking;

namespace WaxRentals.Service.Controllers
{
    public class TrackController : ServiceBase
    {

        private ITelegramNotifier Telegram { get; }
        private ITracker Tracker { get; }

        public TrackController(IDataFactory factory, ITelegramNotifier telegram, ITracker tracker)
            : base(factory)
        {
            Telegram = telegram;
            Tracker = tracker;
        }

        [HttpPost("Error")]
        public async Task<JsonResult> Error([FromBody] ErrorLog log)
        {
            await Factory.Log.Error(log.Exception, log.Error, log.Context);
            return Succeed();
        }

        [HttpPost("Message")]
        public async Task<JsonResult> Message([FromBody] MessageLog log)
        {
            var direction = log.Direction switch
            {
                MessageLogDirection.In  => MessageDirection.In,
                MessageLogDirection.Out => MessageDirection.Out,
                _ => throw new ArgumentOutOfRangeException(nameof(log.Direction), log.Direction, "Unsupported value.")
            };
            await Factory.Log.Message(log.RequestId, log.Url, direction, log.Message);
            return Succeed();
        }

        [HttpPost("Transaction")]
        public JsonResult Transaction([FromBody] TransactionLog log)
        {
            Tracker.Track(log.Description, log.Quantity, log.Coin, log.Earned, log.Spent);
            return Succeed();
        }

        [HttpPost("Notify")]
        public JsonResult Notify([FromBody] string message)
        {
            Telegram.Send(message);
            return Succeed();
        }

    }
}
