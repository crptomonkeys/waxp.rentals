using System;
using System.Threading.Tasks;
using WaxRentals.Data.Entities;
using WaxRentals.Data.Manager;
using BananoAccount = WaxRentals.Banano.Transact.ITransact;

namespace WaxRentals.Processing.Processors
{
    internal class PaymentProcessor : Processor<Payment>
    {

        private BananoAccount Banano { get; }

        public PaymentProcessor(IProcess data, ILog log, BananoAccount banano)
            : base(data, log)
        {
            Banano = banano;
        }

        protected override Func<Task<Payment>> Get => Data.PullNextPayment;
        protected override async Task Process(Payment payment)
        {
            var hash = await Banano.Send(payment.BananoAddress, payment.Banano);
            await Data.ProcessPayment(payment.PaymentId, hash);
        }

    }
}
