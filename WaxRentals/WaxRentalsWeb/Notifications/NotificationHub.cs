﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using WaxRentals.Banano.Transact;
using WaxRentals.Monitoring.Recents;
using WaxRentalsWeb.Data;
using WaxRentalsWeb.Data.Models;

namespace WaxRentalsWeb.Notifications
{
    public class NotificationHub : Hub
    {

        private readonly IDataCache _data;
        private IAppStateMonitor AppState { get; }
        private readonly IHubContext<NotificationHub> _context;
        private readonly IBananoAccountFactory _banano;
        
        // This must be public to work.
        public NotificationHub(
            IDataCache data,
            IAppStateMonitor appState,
            IHubContext<NotificationHub> context,
            IBananoAccountFactory banano)
        {
            _data = data;
            AppState = appState;
            _context = context;
            _banano = banano;

            _data.AppStateChanged += async (_, _) => await NotifyAppState(_context.Clients.All);
            _data.InsightsChanged += async (_, _) => await NotifyInsights(_context.Clients.All);
            AppState.Updated += async (_, _) => await NotifyAppState(_context.Clients.All);
        }

        public async override Task OnConnectedAsync()
        {
            var appState = NotifyAppState(Clients.Caller);
            var recents = NotifyInsights(Clients.Caller);
            await Task.WhenAll(appState, recents);
            await base.OnConnectedAsync();
        }

        #region " Notifications "

        private async Task NotifyAppState(IClientProxy client)
        {
            if (_data.AppState == null || AppState.Value == null)
            {
                return;
            }
            await Notify(client, "AppStateChanged", () => new AppStateModel(_data.AppState, AppState.Value));
        }

        private async Task NotifyInsights(IClientProxy client)
        {
            await Notify(client, "InsightsChanged", () => new InsightsModel(_data.Insights, _banano));
        }

        private async Task Notify<T>(IClientProxy client, string method, Func<T> getData)
        {
            try
            {
                var data = getData();
                await client.SendAsync(method, data);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send {method} notification.");
                Console.WriteLine(ex);
            }
        }

        #endregion

    }
}
