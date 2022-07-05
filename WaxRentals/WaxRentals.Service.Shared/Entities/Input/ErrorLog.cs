using System;

#nullable disable

namespace WaxRentals.Service.Shared.Entities.Input
{
    public class ErrorLog
    {

        public Exception Exception { get; set; }
        public string Error { get; set; }
        public object Context { get; set; }

    }
}
