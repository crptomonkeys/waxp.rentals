namespace WaxRentals.Service.Shared.Entities
{
    public class NewWelcomePackage
    {

        public string Address { get; set; }
        public string Link { get; set; }
        public string Account { get; set; }
        public string Memo { get; set; }

        public NewWelcomePackage(string address, string link, string account, string memo)
        {
            Address = address;
            Link = link;
            Account = account;
            Memo = memo;
        }

    }
}
