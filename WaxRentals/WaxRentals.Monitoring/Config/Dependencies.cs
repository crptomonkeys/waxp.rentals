using System.Net.Http;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Telegram.Bot;
using Telegram.Bot.Types;
using WaxRentals.Data.Manager;
using WaxRentals.Monitoring.Notifications;
using static WaxRentals.Monitoring.Config.Constants;
using File = System.IO.File;

namespace WaxRentals.Monitoring.Config
{
    public static class Dependencies
    {

        public static void AddDependencies(this IServiceCollection services)
        {
            var telegram = JObject.Parse(File.ReadAllText(Secrets.TelegramInfo)).ToObject<TelegramInfo>();

            services.AddSingleton<ITelegramNotifier>(provider =>
                new TelegramNotifier(
                    new TelegramBotClient(telegram.Token, new HttpClient { Timeout = QuickTimeout }),
                    new ChatId(telegram.TargetChat),
                    provider.GetRequiredService<ILog>()
                )
            );
        }

    }
}
