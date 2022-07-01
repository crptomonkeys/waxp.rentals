using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WaxRentals.Banano.Transact;
using WaxRentals.Data.Manager;
using WaxRentals.Service.Shared.Connectors;
using WaxRentals.Service.Shared.Entities;
using WaxRentalsWeb.Config;
using WaxRentalsWeb.Data;
using WaxRentalsWeb.Data.Models;
using static WaxRentalsWeb.Config.Constants;

namespace WaxRentalsWeb.Pages
{
    public class IndexModel : PageModel
    {

        public PageLoadModel InitialPage { get; private set; }

        private IAppService Service { get; }
        private IDataFactory Data { get; }
        private IBananoAccountFactory Banano { get; }
        private ITrackService Track { get; }

        public IndexModel(IAppService service, IDataFactory data, IBananoAccountFactory banano, ITrackService track)
        {
            Service = service;
            Data = data;
            Banano = banano;
            Track = track;
        }

        public void OnGet()
        {
            InitialPage = TempData.Get<PageLoadModel>("InitialPage") ?? new PageLoadModel { Name = "default" };
        }

        public async Task<JsonResult> OnPostAsync(RentalInput input)
        {
            try
            {
                var state = (await Service.State()).Value;
                var (valid, error) = Validate(input, state);
                if (!valid)
                {
                    return Fail(error);
                }

                var cost = (input.CPU + input.NET) * input.Days * state.WaxRentPriceInBanano;
                var id = await Data.Insert.OpenRental(input.Account, RentalDays(input.Days), input.CPU, input.NET, decimal.Round(cost, 4));
                var account = Banano.BuildAccount(id);
                await Track.Notify($"Starting rental process for {input.Account}.");
                return Succeed(account.Address);
            }
            catch (Exception ex)
            {
                try
                {
                    await Track.Error(ex);
                    return Fail(ex.Message);
                }
                catch
                {
                    return Fail("Unknown error.");
                }
            }
        }

        private (bool valid, string error) Validate(RentalInput input, AppState state)
        {
            if (!ModelState.IsValid)
            {
                var (name, entry) = ModelState.First(kvp => kvp.Value.Errors.Any());
                var errors = string.Join(" ", entry.Errors.Select(e => e.ErrorMessage));
                return (false, $"{name}: {errors}");
            }
            else if (input.CPU + input.NET < state.WaxMinimumRent)
            {
                return (false, $"Must rent at least {state.WaxMinimumRent} WAX.");
            }
            else if (input.CPU + input.NET > state.WaxMaximumRent)
            {
                return (false, $"Cannot rent more than {state.WaxMaximumRent} WAX in one transaction right now.");
            }
            else if (input.Days < 1)
            {
                return (false, $"Must rent for at least one day.");
            }
            return (true, null);
        }

        private static int RentalDays(uint input)
        {
            int days = (int)input;
            return (days >= Calculations.DaysDoubleThreshold) ? (days * 2) : days;
        }

        private static JsonResult Succeed(string address) => new(RentalResult.Succeed(address));
        private static JsonResult Fail(string error) => new(RentalResult.Fail(error));

    }
}
