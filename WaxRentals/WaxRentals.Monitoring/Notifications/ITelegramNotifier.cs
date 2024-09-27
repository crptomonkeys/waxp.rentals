namespace WaxRentals.Monitoring.Notifications
{
    public interface ITelegramNotifier
    {

        void Send(string message);

    }
}
