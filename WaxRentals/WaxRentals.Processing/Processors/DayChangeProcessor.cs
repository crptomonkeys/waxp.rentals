using System;
using System.Threading.Tasks;
using WaxRentals.Service.Shared.Connectors;
using WaxRentals.Service.Shared.Entities;

namespace WaxRentals.Processing.Processors
{
    internal class DayChangeProcessor : Processor<Result>
    {

        private IWaxService Wax { get; }

        public DayChangeProcessor(ITrackService track, IWaxService wax)
            : base(track)
        {
            Wax = wax;
        }

        protected override Func<Task<Result>> Get => Wax.Sweep;
        protected override Task<bool> Process(Result result)
        {
            // no-op
            return Task.FromResult(false);
        }

    }
}
