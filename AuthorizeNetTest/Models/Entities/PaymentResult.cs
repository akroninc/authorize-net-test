using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AuthorizeNetTest.Models.Entities
{
    public class PaymentResult
    {
        public bool IsSuccessful { get { return ResultCode == 0 && ErrorCode == null; } }
        public int ResultCode { get; set; }

        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }

        public string AuthCode { get; set; }
        public string Result { get; internal set; }
    }
}