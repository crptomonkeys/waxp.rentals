﻿using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WaxRentals.Banano.Transact;
using WaxRentals.Data.Manager;
using WaxRentals.Monitoring.Notifications;
using WaxRentalsWeb.Data;
using WaxRentalsWeb.Data.Models;
using static WaxRentals.Waxp.Config.Constants;
using static WaxRentalsWeb.Data.WelcomePackageResult;

namespace WaxRentalsWeb.Pages
{
    public class WelcomePackageModel : PageModel
    {

        private readonly IDataCache _cache;
        private readonly IDataFactory _data;
        private readonly IBananoAccountFactory _banano;
        private readonly ITelegramNotifier _telegram;

        public WelcomePackageModel(IDataCache cache, IDataFactory data, IBananoAccountFactory banano, ITelegramNotifier telegram)
        {
            _cache = cache;
            _data = data;
            _banano = banano;
            _telegram = telegram;
        }

        public async Task<JsonResult> OnPostAsync(string memo)
        {
            try
            {
                // Filter invalid memos.
                if (string.IsNullOrWhiteSpace(memo) || !Regex.IsMatch(memo, Protocol.NewUser.MemoRegex))
                {
                    return Fail("Please check that the memo provided is correct.");
                }

                var cost = _cache.AppState.BananoWelcomePackagePrice;
                if (cost == 0)
                {
                    return Fail("Something went wrong; please try again in a few minutes.");
                }

                var id = await _data.Insert.OpenWelcomePackage(Protocol.NewUser.Account, memo, Protocol.NewUser.OpenWax, cost);
                var account = _banano.BuildWelcomeAccount((uint)id);
                _telegram.Send($"Starting welcome package process for {memo}.");
                return Succeed(new WelcomePackageDetail
                {
                    Address = new BananoAddressModel(account.Address),
                    Link = account.BuildLink(cost),
                    Account = Protocol.NewUser.Account,
                    Memo = memo
                });
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

        protected JsonResult Succeed(WelcomePackageDetail detail) => new JsonResult(WelcomePackageResult.Succeed(detail));
        protected JsonResult Fail(string error) => new JsonResult(WelcomePackageResult.Fail(error));

    }
}