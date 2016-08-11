using System.Collections.Generic;
using System.Web.Mvc;
using AuthorizeNetTest.Models.Entities;
using AuthorizeNetTest.Utilities;

namespace AuthorizeNetTest.Models.ViewModels
{
    public class PaymentViewModel : ViewModelBase
    {
        public PaymentViewModel()
        {
            Payment = new Payment();
            Billing = new BillingInfo();

            StateList = ListHelper.GetStateList();
            CentsList = ListHelper.GetCentsList();
        }

        public Payment Payment { get; set; }
        public BillingInfo Billing { get; set; }
        public PaymentResult PaymentResult { get; set; }
        public IEnumerable<SelectListItem> StateList { get; internal set; }
        public IEnumerable<SelectListItem> CentsList { get; private set; }
    }
}