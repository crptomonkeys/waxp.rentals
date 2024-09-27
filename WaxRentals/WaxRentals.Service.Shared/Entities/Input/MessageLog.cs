using System;

#nullable disable

namespace WaxRentals.Service.Shared.Entities.Input
{
    public class MessageLog
    {

        public Guid RequestId { get; set; }
        public string Url { get; set; }
        public MessageLogDirection Direction { get; set; }
        public string Message{ get; set; }

    }
}
