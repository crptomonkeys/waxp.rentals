﻿#nullable disable

namespace WaxRentals.Service.Shared.Entities.Input
{
    public class StakeInput
    {

        public string Source { get; set; }
        public string Target { get; set; }
        public int Cpu { get; set; }
        public int Net { get; set; }
        public int? Days { get; set; }

    }
}
