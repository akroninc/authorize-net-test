using AuthorizeNetTest.Models.ViewModels;
using AuthorizeNetTest.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AuthorizeNetTest.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            /*
                Visa: 4111111111111111
                Discover: 6011111111111117
                MasterCard: 5111111111111118
                Maestro: 5018111111111112
                JCB: 3511111111111119
                Union Pay: 6211111111111111
                American Express: 371111111111114
                Diners Club: 38111111111119
             */
            var model = new PaymentViewModel();
            return View(model);
        }

        [HttpPost]
        public ActionResult Payment(PaymentViewModel model)
        {
            var paymentInfo = model.Payment;
            var billingInfo = model.Billing;

            //int paymentAmountCents = 0;
            //if (int.TryParse(paymentInfo.AmountCents, out paymentAmountCents))
            var paymentAmount = 1.0m + (paymentInfo.AmountCents / 100.0m);

            var paymentHelper = new Utilities.PaymentHelper();


            model.PaymentResult = paymentHelper.ChargeCard(paymentInfo.CreditCardNumber,
                                                         paymentInfo.ExpiryMonth,
                                                         paymentInfo.ExpiryYear,
                                                         paymentInfo.CVC,
                                                         paymentAmount,
                                                         billingInfo.FirstName,
                                                         billingInfo.LastName,
                                                         billingInfo.Address,
                                                         billingInfo.City,
                                                         billingInfo.State,
                                                         billingInfo.ZipCode);

            if (model.PaymentResult.IsSuccessful)
                return RedirectToAction("Confirmation", new { authCode = model.PaymentResult.AuthCode });

            return View("Index", model);
        }
        
        public ActionResult Confirmation(string authCode)
        {
            if (string.IsNullOrWhiteSpace(authCode))
                return RedirectToAction("Index");

            var model = new ConfirmationViewModel { AuthCode = authCode };
            return View(model);
        }
    }
}