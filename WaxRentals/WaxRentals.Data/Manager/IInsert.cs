﻿using System.Threading.Tasks;
using WaxRentals.Data.Entities;

namespace WaxRentals.Data.Manager
{
    public interface IInsert
    {

        Task<int> OpenRental(string account, int days, decimal cpu, decimal net, decimal banano);
        Task<bool> OpenPurchase(decimal wax, string transaction, string bananoAddress, decimal banano, Status status);
        Task<int> OpenWelcomePackage(string account, string memo, decimal wax, decimal banano);

    }
}
