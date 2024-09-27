using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using WaxRentalsWeb.Files;
using WaxRentalsWeb.Monitoring;

namespace WaxRentalsWeb.Notifications
{
    public class NotificationHub : Hub
    {

        private IAppStateMonitor AppState { get; }
        private IAppInsightsMonitor AppInsights { get; }
        private SiteMessageMonitor SiteMessage { get; }
        
        // This must be public to work.
        public NotificationHub(
            IAppStateMonitor appState,
            IAppInsightsMonitor appInsights,
            SiteMessageMonitor siteMessage,
            IHubContext<NotificationHub> hub)
        {
            AppState = appState;
            AppState.Updated += async (_, _) => await NotifyState(hub.Clients.All);
            
            AppInsights = appInsights;
            AppInsights.Updated += async (_, _) => await NotifyInsights(hub.Clients.All);

            SiteMessage = siteMessage;
            SiteMessage.Updated += async (_, _) => await NotifyAlert(hub.Clients.All);
            SiteMessage.Initialize();
        }

        public async override Task OnConnectedAsync()
        {
            var appState = NotifyState(Clients.Caller);
            var recents = NotifyInsights(Clients.Caller);
            var alert = NotifyAlert(Clients.Caller);
            await Task.WhenAll(appState, recents, alert);
            await base.OnConnectedAsync();
        }

        #region " Notifications "

        private async Task NotifyState(IClientProxy client)
        {
            if (AppState.Value == null)
            {
                return;
            }
            await Notify(client, "StateChanged", () => AppState.Value);
        }

        private async Task NotifyInsights(IClientProxy client)
        {
            if (AppInsights.Value == null)
            {
                return;
            }
            await Notify(client, "InsightsChanged", () => AppInsights.Value);
        }

        private async Task NotifyAlert(IClientProxy client)
        {
            await Notify(client, "AlertChanged", () => SiteMessage.Contents);
        }

        private static async Task Notify<T>(IClientProxy client, string method, Func<T> getData)
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
