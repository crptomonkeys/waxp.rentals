using System;
using System.Threading.Tasks;
using WaxRentals.Service.Shared.Entities;
using WaxRentals.Service.Shared.Entities.Input;

namespace WaxRentals.Service.Shared.Connectors
{
    public interface IRentalService
    {
        Task<Result<string>> New(RentalInput input);
    }

    internal class RentalService : Connector, IRentalService
    {

        public RentalService(Uri baseUrl, ITrackService log) : base(baseUrl, log) { }

        public async Task<Result<string>> New(RentalInput input)
        {
            return await Post<string>("New", input);
        }

    }
}
