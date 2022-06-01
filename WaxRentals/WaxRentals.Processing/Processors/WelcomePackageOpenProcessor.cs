using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WaxRentals.Banano.Transact;
using WaxRentals.Data.Entities;
using WaxRentals.Data.Manager;
using WaxRentals.Monitoring.Notifications;

namespace WaxRentals.Processing.Processors
{
    internal class WelcomePackageOpenProcessor : Processor<IEnumerable<WelcomePackage>>
    {

        protected override bool ProcessMultiplePerTick => false;

        private IBananoAccountFactory Banano { get; }
        private ITelegramNotifier Telegram { get; }

        public WelcomePackageOpenProcessor(IDataFactory factory, IBananoAccountFactory banano, ITelegramNotifier telegram)
            : base(factory)
        {
            Banano = banano;
            Telegram = telegram;
        }

        protected override Func<Task<IEnumerable<WelcomePackage>>> Get => Factory.Process.PullNewWelcomePackages;
        protected async override Task Process(IEnumerable<WelcomePackage> packages)
        {
            var tasks = packages.Select(Process);
            await Task.WhenAll(tasks);
        }

        private async Task Process(WelcomePackage package)
        {
            try
            {
                var account = Banano.BuildWelcomeAccount((uint)package.PackageId);
                var balance = await account.GetBalance();
                if (balance >= package.Banano)
                {
                    await Factory.Process.ProcessWelcomePackagePayment(package.PackageId);
                    Telegram.Send($"Received welcome package payment for {package.Memo}.");
                }
            }
            catch (Exception ex)
            {
                await Factory.Log.Error(ex, context: package);
            }
        }

    }
}
