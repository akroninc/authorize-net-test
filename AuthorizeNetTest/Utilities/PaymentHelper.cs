
using AuthorizeNet.Api.Controllers;
using AuthorizeNet.Api.Contracts.V1;
using AuthorizeNet.Api.Controllers.Bases;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text.RegularExpressions;
using AuthorizeNetTest.Models.Entities;

namespace AuthorizeNetTest.Utilities
{
    public class PaymentHelper
    {
        private string ApiLoginID = ConfigHelper.ApiLoginId; // "8UKwS4kQ9Ku";
        private string ApiTransactionKey = ConfigHelper.ApiTransactionKey; // "9x6S25Fh895mh2VQ";
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();

        public PaymentHelper()
        {
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.RunEnvironment = ConfigHelper.UseApiSandbox 
                                                                             ? AuthorizeNet.Environment.SANDBOX
                                                                             : AuthorizeNet.Environment.PRODUCTION;

            // define the merchant information (authentication / transaction id)
            ApiOperationBase<ANetApiRequest, ANetApiResponse>.MerchantAuthentication = new merchantAuthenticationType()
            {
                name = ApiLoginID,
                ItemElementName = ItemChoiceType.transactionKey,
                Item = ApiTransactionKey
            };
        }

        public PaymentResult ChargeCard(string cardNumber, int expirationMonth, int expirationYear, string cvc, decimal amount, string firstName = null, string lastName = null, string address = null, string city = null, string state = null, string zip = null)
        {
            try
            {
                var expirationDate = string.Format("{0}{1}",
                                                    expirationMonth.ToString().PadLeft(2, '0'),
                                                    expirationYear.ToString().Substring(2, 2));

                cardNumber = RemoveNonNumeric(cardNumber);

                var creditCard = new creditCardType
                {
                    cardNumber = cardNumber,
                    expirationDate = expirationDate,
                    cardCode = cvc
                };

                customerAddressType billingAddress = null,
                                    shippingAddress = null;
                if (!string.IsNullOrWhiteSpace(firstName) ||
                    !string.IsNullOrWhiteSpace(lastName) ||
                    !string.IsNullOrWhiteSpace(address) ||
                    !string.IsNullOrWhiteSpace(city) ||
                    !string.IsNullOrWhiteSpace(state) ||
                    !string.IsNullOrWhiteSpace(zip))
                {
                    billingAddress = new customerAddressType
                    {
                        firstName = string.IsNullOrWhiteSpace(firstName) ? null : firstName,
                        lastName = string.IsNullOrWhiteSpace(lastName) ? null : lastName,
                        address = string.IsNullOrWhiteSpace(address) ? null : address,
                        city = string.IsNullOrWhiteSpace(city) ? null : city,
                        state = string.IsNullOrWhiteSpace(state) ? null : state,
                        zip = string.IsNullOrWhiteSpace(zip) ? null : zip
                    };

                    shippingAddress = billingAddress; // TODO: Allow for alternate shipping address
                }

                //standard api call to retrieve response
                var paymentType = new paymentType { Item = creditCard };

                var transactionRequest = new transactionRequestType
                {
                    transactionType = transactionTypeEnum.authCaptureTransaction.ToString(),    // charge the card

                    amount = amount,
                    payment = paymentType,
                    billTo = billingAddress,
                    shipTo = shippingAddress
                };

                var request = new createTransactionRequest { transactionRequest = transactionRequest };

                // instantiate the contoller that will call the service
                var controller = new createTransactionController(request);
                controller.Execute();

                // get the response from the service (errors contained if any)
                var response = controller.GetApiResponse();

                if (response == null)
                {
                    var errorResponse = controller.GetErrorResponse();
                    if (errorResponse == null)
                    {
                        logger.Error("API Error - No Response from this Request:\r\n{0}", request.ToJson());

                        return new PaymentResult
                        {
                            ResultCode = -1,
                            Result = "NO RESPONSE",
                            ErrorCode = "-1",
                            ErrorMessage = "NO REPONSE OR ERROR FROM API"
                        };
                    }
                    else
                    {
                        logger.Error("API Error\r\n{0}", errorResponse.ToJson());

                        return new PaymentResult
                        {
                            ResultCode = (int)errorResponse.messages.resultCode,
                            Result = errorResponse.messages.resultCode.ToString(),
                            ErrorCode = errorResponse.messages.message[0].code,
                            ErrorMessage = errorResponse.messages.message[0].text
                        };
                    }
                }
                else
                {
                    if (response.transactionResponse.errors == null)
                        logger.Debug("Transaction Successful\r\n{0}", response.ToJson());
                    else
                        logger.Error("Transaction Error\r\n{0}", response.ToJson());

                    return new PaymentResult
                    {
                        AuthCode = response.transactionResponse.authCode,
                        ResultCode = (int)response.messages.resultCode,
                        Result = response.messages.resultCode.ToString(),
                        ErrorCode = response.transactionResponse.errors == null 
                                  ? null 
                                  : response.transactionResponse.errors[0].errorCode,
                        ErrorMessage = response.transactionResponse.errors == null 
                                     ? null 
                                     : response.transactionResponse.errors[0].errorText
                    };
                }
            }
            catch (Exception ex)
            {
                // LOG EXCEPTION
                logger.Error(ex, "Unhandled Exception!");

                return new PaymentResult
                {
                    ResultCode = -99,
                    Result = string.Format("UNHANDLED EXCEPTION: {0}", ex.ToString()),
                    ErrorCode = "EXCEPTION",
                    ErrorMessage = ex.Message
                };
            }
        }

        private string RemoveNonNumeric(string str)
        {
            var digitsOnly = new Regex(@"[^\d]");
            return digitsOnly.Replace(str, "");
        }
    }
}