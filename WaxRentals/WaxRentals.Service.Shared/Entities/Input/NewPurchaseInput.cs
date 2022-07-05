namespace WaxRentals.Service.Shared.Entities.Input
{
    public class NewPurchaseInput
    {

        public decimal Amount { get; set; }
        public string Transaction { get; set; }
        public string BananoPaymentAddress { get; set; }
        public decimal Banano { get; set; }
        public Status Status { get; set; }

        public NewPurchaseInput(decimal amount, string transaction, string bananoPaymentAddress, decimal banano, Status status)
        {
            Amount = amount;
            Transaction = transaction;
            BananoPaymentAddress = bananoPaymentAddress;
            Banano = banano;
            Status = status;
        }

    }
}
