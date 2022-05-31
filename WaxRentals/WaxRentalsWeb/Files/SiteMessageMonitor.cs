using WaxRentals.Monitoring;

namespace WaxRentalsWeb.Files
{
    internal class SiteMessageMonitor : FileMonitor
    {

        public SiteMessageMonitor() : base("/run/files/site-message") { }

    }
}
