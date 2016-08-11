using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AuthorizeNetTest.Models.ViewModels
{
    public class ConfirmationViewModel : ViewModelBase
    {
        public string AuthCode { get; set; }
    }
}