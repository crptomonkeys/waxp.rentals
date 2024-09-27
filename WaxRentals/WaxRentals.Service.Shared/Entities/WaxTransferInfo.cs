﻿#nullable disable

namespace WaxRentals.Service.Shared.Entities
{
    public class WaxTransferInfo
    {

        public string Source { get; set; }
        public string Transaction { get; set; }
        public decimal Amount { get; set; }
        public string BananoPaymentAddress { get; set; }
        public bool SkipPayment { get; set; }

    }
}
