namespace WaxRentals.Service.Shared.Entities.Input
{
    public struct RentalInput
    {

        public string Account { get; set; }
        public int Days { get; set; }
        public decimal Cpu { get; set; }
        public decimal Net { get; set; }
        public bool Free { get; set; }

    }
}
