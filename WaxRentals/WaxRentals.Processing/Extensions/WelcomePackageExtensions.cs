using System;
using WaxRentals.Data.Entities;

namespace WaxRentals.Processing.Extensions
{
    internal static class WelcomePackageExtensions
    {

        public static string MemoToAccount(this WelcomePackage @this)
        {
            return @this.Memo.Replace("DOT", ".", StringComparison.Ordinal);
        }

    }
}
