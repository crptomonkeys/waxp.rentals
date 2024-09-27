using System;
using System.Threading.Tasks;
using WaxRentals.Service.Shared.Entities;
using WaxRentals.Service.Shared.Entities.Input;

namespace WaxRentals.Service.Shared.Connectors
{
    public interface ITrackService
    {
        Task<Result> Error(Exception exception);
        Task<Result> Error(ErrorLog log);
        Task<Result> ClearOlderRecords();
        Task<Result> Message(MessageLog log);
        Task<Result> Transaction(TransactionLog log);
        Task<Result> Notify(string message);
    }

    internal class TrackService : Connector, ITrackService
    {

        public TrackService(Uri baseUrl) : base(baseUrl) { }

        public async Task<Result> Error(Exception exception)
        {
            return await Error(new ErrorLog { Exception = exception });
        }

        public async Task<Result> Error(ErrorLog log)
        {
            return await Post("Error", log);
        }

        public async Task<Result> Message(MessageLog log)
        {
            return await Post("Message", log);
        }

        public async Task<Result> ClearOlderRecords()
        {
            return await Post("ClearOlderRecords");
        }

        public async Task<Result> Transaction(TransactionLog log)
        {
            return await Post("Transaction", log);
        }

        public async Task<Result> Notify(string message)
        {
            return await Post("Notify", message);
        }

    }
}
