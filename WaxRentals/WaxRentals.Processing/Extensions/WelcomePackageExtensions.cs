using System;
using WaxRentals.Service.Shared.Entities;

namespace WaxRentals.Processing.Extensions
{
    internal static class WelcomePackageExtensions
    {

        public static string MemoToAccount(this WelcomePackageInfo @this)
        {
            return @this.Memo.Replace("DOT", ".", StringComparison.Ordinal);
        }

    }
}
