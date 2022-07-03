using System;
using System.Threading.Tasks;
using WaxRentals.Service.Shared.Entities;

#nullable disable

namespace WaxRentals.Service.Shared.Connectors
{
    public interface IWaxService
    {

    }

    internal class WaxService : Connector, IWaxService
    {

        public WaxService(Uri baseUrl, ITrackService log) : base(baseUrl, log) { }

    }
}
