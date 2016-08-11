using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace AuthorizeNetTest.Models.Entities
{
    public class Payment
    {
        [Required]
        [DataType(DataType.CreditCard, ErrorMessage = "Invalid Credit Card Number")]
        public string CreditCardNumber { get; set; }


        [Required]
        public string CVC { get; set; }

        [Required]
        public string ExpiryDate { get; set; }

        public int ExpiryYear { get; set; }

        public int ExpiryMonth { get; set; }

        public int AmountCents { get; set; }
    }
}