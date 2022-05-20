using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WaxRentals.Banano.Transact;
using WaxRentals.Data.Manager;
using WaxRentalsWeb.Data;

namespace WaxRentalsWeb.Pages
{
    public class IndexModel : PageModel
    {

        private readonly IDataCache _cache;
        private readonly IDataFactory _data;
        private readonly IBananoAccountFactory _banano;

        public IndexModel(IDataCache cache, IDataFactory data, IBananoAccountFactory banano)
        {
            _cache = cache;
            _data = data;
            _banano = banano;
        }

        public void OnGet()
        {
            // Just return the page.
        }

        public async Task<JsonResult> OnPostAsync(RentalInput input)
        {
            try
            {
                var (valid, error) = Validate(input);
                if (!valid)
                {
                    return Fail(error);
                }

                var cost = (input.CPU + input.NET) * input.Days * _cache.AppState.WaxRentPriceInBanano;
                var id = await _data.Insert.OpenRental(input.Account, (int)input.Days, input.CPU, input.NET, decimal.Round(cost, 4));
                var account = _banano.BuildAccount((uint)id);
                return Succeed(account.Address);
            }
            catch (Exception ex)
            {
                try
                {
                    await _data.Log.Error(ex);
                    return Fail(ex.Message);
                }
                catch
                {
                    return Fail("Unknown error.");
                }
            }
        }

        private (bool valid, string error) Validate(RentalInput input)
        {
            if (!ModelState.IsValid)
            {
                var (name, state) = ModelState.First(kvp => kvp.Value.Errors.Any());
                var errors = string.Join(" ", state.Errors.Select(e => e.ErrorMessage));
                return (false, $"{name}: {errors}");
            }
            else if (input.CPU + input.NET < _cache.AppState.WaxMinimumRent)
            {
                return (false, $"Must rent at least {_cache.AppState.WaxMinimumRent} WAX.");
            }
            else if (input.CPU + input.NET > _cache.AppState.WaxMaximumRent)
            {
                return (false, $"Cannot rent more than {_cache.AppState.WaxMaximumRent} WAX in one transaction right now.");
            }
            else if (input.Days < 1)
            {
                return (false, $"Must rent for at least one day.");
            }
            return (true, null);
        }

        private JsonResult Fail(string error)
        {
            return new JsonResult(RentalResult.Fail(error));
        }

        private JsonResult Succeed(string address)
        {
            return new JsonResult(RentalResult.Succeed(address));
        }

    }
}
