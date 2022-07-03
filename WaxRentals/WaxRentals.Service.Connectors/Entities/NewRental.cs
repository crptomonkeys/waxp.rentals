namespace WaxRentals.Service.Shared.Entities
{
    public class NewRental
    {

        public int Id { get; set; }
        public string Address { get; set; }

        public NewRental(int id, string address)
        {
            Id = id;
            Address = address;
        }

    }
}
