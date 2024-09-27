using System;
using Telegram.Bot;
using Telegram.Bot.Types;
using WaxRentals.Data.Manager;

namespace WaxRentals.Monitoring.Notifications
{
    internal class TelegramNotifier : ITelegramNotifier
    {

        private ITelegramBotClient Telegram { get; }
        private ChatId TargetChat { get; }
        private ILog Log { get; }

        public TelegramNotifier(ITelegramBotClient telegram, ChatId targetChat, ILog log)
        {
            Telegram = telegram;
            TargetChat = targetChat;
            Log = log;
        }

        public async void Send(string message)
        {
            try
            {
                await Telegram.SendTextMessageAsync(TargetChat, message);
            }
            catch (Exception ex)
            {
                await Log.Error(ex, context: new { message });
            }
        }

    }
}
