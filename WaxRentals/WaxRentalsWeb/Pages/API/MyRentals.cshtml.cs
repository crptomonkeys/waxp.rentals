using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WaxRentals.Banano.Transact;
using WaxRentals.Data.Entities;
using WaxRentals.Data.Manager;
using WaxRentalsWeb.Data.Models;
using static WaxRentals.Waxp.Config.Constants;

namespace WaxRentalsWeb.Pages
{
    public class MyRentalsModel : PageModel
    {

        private readonly IDataFactory _data;
        private readonly IBananoAccountFactory _banano;

        public MyRentalsModel(IDataFactory data, IBananoAccountFactory banano)
        {
            _data = data;
            _banano = banano;
        }

        public JsonResult OnGet(string account)
        {
            // Filter invalid accounts.
            if (!string.IsNullOrWhiteSpace(account) && Regex.IsMatch(account, Protocol.WaxAddressRegex))
            {
                return Process(() => _data.Explore.GetRentalsByWaxAccount(account));
            }
            return new JsonResult(Enumerable.Empty<TrackedRentalModel>());
        }

        public JsonResult OnPost([FromBody] IEnumerable<string> addresses)
        {
            // Filter invalid addresses.
            addresses = addresses.Where(address => !string.IsNullOrWhiteSpace(address) &&
                                                   Regex.IsMatch(address, Protocol.BananoAddressRegex));
            return Process(() => _data.Explore.GetRentalsByBananoAddresses(addresses));
        }

        private JsonResult Process(Func<IEnumerable<Rental>> get)
        {
            try
            {
                var results = get();
                var mapped = results.Select(rental => new TrackedRentalModel(rental, _banano));
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
            catch (Exception ex)
            {
                _data.Log.Error(ex);
                return new JsonResult(Enumerable.Empty<TrackedRentalModel>());
            }
        }

    }
}
