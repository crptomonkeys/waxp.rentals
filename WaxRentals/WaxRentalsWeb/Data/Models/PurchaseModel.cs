﻿using WaxRentals.Data.Entities;

namespace WaxRentalsWeb.Data.Models
{
    public class PurchaseModel
    {

        public decimal Wax { get; }
        public decimal Banano { get; }
        public string WaxTransaction { get; }
        public string BananoTransaction { get; }

        public PurchaseModel(Purchase purchase)
        {
            Wax               = purchase.Wax;
            Banano            = purchase.Banano;
            WaxTransaction    = purchase.WaxTransaction;
            BananoTransaction = purchase.BananoTransaction;
        }

    }
}
