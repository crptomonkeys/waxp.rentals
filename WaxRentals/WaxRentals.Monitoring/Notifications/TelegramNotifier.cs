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
        private IDataFactory Factory { get; }

        public TelegramNotifier(ITelegramBotClient telegram, ChatId targetChat, IDataFactory factory)
        {
            Telegram = telegram;
            TargetChat = targetChat;
            Factory = factory;
        }

        public async void Send(string message)
        {
            try
            {
                await Telegram.SendTextMessageAsync(TargetChat, message);
            }
            catch (Exception ex)
            {
                await Factory.Log.Error(ex, context: new { message });
            }
        }

    }
}
