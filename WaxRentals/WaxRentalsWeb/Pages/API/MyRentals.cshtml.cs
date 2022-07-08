using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WaxRentals.Service.Shared.Connectors;
using WaxRentals.Service.Shared.Entities;
using WaxRentalsWeb.Data.Models;
using static WaxRentals.Service.Shared.Config.Constants;

namespace WaxRentalsWeb.Pages
{
    public class MyRentalsModel : PageModel
    {

        private IRentalService Rentals { get; }
        private ITrackService Track { get; }

        public MyRentalsModel(IRentalService rentals)
        {
            Rentals = rentals;
        }

        public async Task<JsonResult> OnGet(string account)
        {
            // Filter invalid accounts.
            if (!string.IsNullOrWhiteSpace(account) && Regex.IsMatch(account, Wax.Protocol.AccountRegex))
            {
                return await Process(async () => await Rentals.ByWaxAccount(account));
            }
            return new JsonResult(Enumerable.Empty<TrackedRentalModel>());
        }

        public async Task<JsonResult> OnPost([FromBody] IEnumerable<string> addresses)
        {
            // Filter invalid addresses.
            addresses = addresses.Where(address => !string.IsNullOrWhiteSpace(address) &&
                                                   Regex.IsMatch(address, Banano.Protocol.AddressRegex));
            return await Process(async () => await Rentals.ByBananoAddresses(addresses));
        }

        private async Task<JsonResult> Process(Func<Task<Result<IEnumerable<RentalInfo>>>> get)
        {
            try
            {
                var result = await get();
                if (result.Success)
                {
                    var mapped = result.Value.Select(rental => new TrackedRentalModel(rental));
                    var grouped = mapped.GroupBy(rental => rental.Status)
                                        .ToDictionary(g => g.Key, g => g.OrderBy(rental => rental.Expires).AsEnumerable());
                    // Make sure every status is represented, because making new arrays in Vue messes things up.
                    foreach (var status in Enum.GetValues<Status>())
                    {
                        if (!grouped.ContainsKey(status))
                        {
                            grouped.Add(status, Enumerable.Empty<TrackedRentalModel>());
                        }
                    }
                    return new JsonResult(grouped);
                }
            }
            catch (Exception ex)
            {
                await Track.Error(ex);
            }
            return new JsonResult(Enumerable.Empty<TrackedRentalModel>());
        }

    }
}
