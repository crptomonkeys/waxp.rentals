using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using WaxRentals.Banano.Transact;
using WaxRentals.Monitoring.Recents;
using WaxRentalsWeb.Data;
using WaxRentalsWeb.Data.Models;
using WaxRentalsWeb.Files;

namespace WaxRentalsWeb.Notifications
{
    public class NotificationHub : Hub
    {

        private readonly IDataCache _data;
        private IAppStateMonitor AppState { get; }
        private IBananoAccountFactory Banano { get; }

        private volatile string _siteMessage;
        
        // This must be public to work.
        public NotificationHub(
            IDataCache data,
            IAppStateMonitor appState,
            SiteMessageMonitor siteMessageMonitor,
            IHubContext<NotificationHub> hub,
            IBananoAccountFactory banano)
        {
            _data = data;
            AppState = appState;
            Banano = banano;

            _data.InsightsChanged += async (_, _) => await NotifyInsights(hub.Clients.All);

            AppState.Updated += async (_, _) => await NotifyAppState(hub.Clients.All);
            
            siteMessageMonitor.Updated += async (_, contents) =>
            {
                _siteMessage = contents;
                await NotifyInsights(hub.Clients.All);
            };
            siteMessageMonitor.Initialize();
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
            if (AppState.Value == null)
            {
                return;
            }
            await Notify(client, "AppStateChanged", () => new AppStateModel(AppState.Value, _siteMessage));
        }

        private async Task NotifyInsights(IClientProxy client)
        {
            await Notify(client, "InsightsChanged", () => new InsightsModel(_data.Insights, Banano));
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
