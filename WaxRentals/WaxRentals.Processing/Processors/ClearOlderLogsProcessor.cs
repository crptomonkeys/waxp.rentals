using System;
using System.Threading.Tasks;
using WaxRentals.Service.Shared.Connectors;
using WaxRentals.Service.Shared.Entities;

namespace WaxRentals.Processing.Processors
{
    internal class ClearOlderLogsProcessor : Processor<Result>
    {

        public ClearOlderLogsProcessor(ITrackService track) : base(track) { }

        protected override Func<Task<Result>> Get => Track.ClearOlderRecords;
        protected override Task<bool> Process(Result result)
        {
            // no-op
            return Task.FromResult(false);
        }

    }
}
