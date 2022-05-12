using System.Threading.Tasks;
using WaxRentals.Banano.Transact;
using WaxRentals.Data.Entities;
using WaxRentals.Data.Manager;

namespace WaxRentals.Processing.Processors
{
    internal class PaymentProcessor : Processor
    {

        private readonly IProcess _data;
        private readonly ITransact _banano;

        public PaymentProcessor(IProcess data, ITransact banano)
        {
            _data = data;
            _banano = banano;
        }

        protected override async Task Run()
        {
            // Process payments one at a time.
            // Revisit if this ends up being too slow.
            var payment = await _data.PullNextPayment();
            while (payment != null)
            {
                await Process(payment);
                payment = await _data.PullNextPayment();
            }
        }

        private async Task Process(Payment payment)
        {
            var hash = await _banano.Send(payment.BananoAddress, payment.Banano);
            await _data.ProcessPayment(payment.PaymentId, hash);
        }

    }
}
