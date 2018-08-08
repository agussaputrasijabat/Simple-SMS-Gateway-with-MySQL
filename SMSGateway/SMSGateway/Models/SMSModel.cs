using System;
using System.Collections.Generic;
using System.Text;

namespace SMSGateway.Models
{
    public class SMSModel
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
    }
}
