using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using WaxRentalsWeb.Data.Models;
using WaxRentalsWeb.Files;
using WaxRentalsWeb.Monitoring;

namespace WaxRentalsWeb.Notifications
{
    public class NotificationHub : Hub
    {

        private IAppStateMonitor AppState { get; }
        private IAppInsightsMonitor AppInsights { get; }

        private LockedString SiteMessage { get; } = new();
        
        // This must be public to work.
        public NotificationHub(
            IAppStateMonitor appState,
            IAppInsightsMonitor appInsights,
            SiteMessageMonitor siteMessageMonitor,
            IHubContext<NotificationHub> hub)
        {
            AppState = appState;
            AppState.Updated += async (_, _) => await NotifyState(hub.Clients.All);
            
            AppInsights = appInsights;
            AppInsights.Updated += async (_, _) => await NotifyInsights(hub.Clients.All);
            
            siteMessageMonitor.Updated += async (_, contents) =>
            {
                SiteMessage.Value = contents;
                await NotifyState(hub.Clients.All);
            };
            siteMessageMonitor.Initialize();
        }

        public async override Task OnConnectedAsync()
        {
            var appState = NotifyState(Clients.Caller);
            var recents = NotifyInsights(Clients.Caller);
            await Task.WhenAll(appState, recents);
            await base.OnConnectedAsync();
        }

        #region " Notifications "

        private async Task NotifyState(IClientProxy client)
        {
            if (AppState.Value == null)
            {
                return;
            }
            await Notify(client, "StateChanged", () => new AppStateModel(AppState.Value, SiteMessage.Value));
        }

        private async Task NotifyInsights(IClientProxy client)
        {
            if (AppInsights.Value == null)
            {
                return;
            }
            await Notify(client, "InsightsChanged", () => new AppInsightsModel(AppInsights.Value));
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
