using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using WaxRentalsWeb.Data;

namespace WaxRentalsWeb.Notifications
{
    public class NotificationHub : Hub
    {

        private readonly IDataCache _data;
        private readonly IHubContext<NotificationHub> _context;

        // This must be public to work.
        public NotificationHub(
            IDataCache data,
            IHubContext<NotificationHub> context)
        {
            _data = data;
            _context = context;

            _data.AppStateChanged += async (_, _) => await NotifyAppState(_context.Clients.All);
        }

        public async override Task OnConnectedAsync()
        {
            var appState = NotifyAppState(Clients.Caller);
            await Task.WhenAll(appState);
            await base.OnConnectedAsync();
        }

        #region " Notifications "

        private async Task NotifyAppState(IClientProxy client)
        {
            await Notify(client, "AppStateChanged", () => _data.AppState);
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
