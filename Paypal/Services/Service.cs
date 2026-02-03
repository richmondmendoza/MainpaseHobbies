using Paypal.Interfaces;
using PayPalCheckoutSdk.Core;
using PayPalCheckoutSdk.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Paypal.Services
{
    public class Service : IServices
    {
        private PayPalEnvironment _environment;
        private PayPalHttpClient _client;

        public Service(bool isLive = false)
        {
            if (isLive)
            {
                _environment = new LiveEnvironment(PaypalModel.ClientId, PaypalModel.SecretId);
            }
            else
            {
                _environment = new SandboxEnvironment(PaypalModel.ClientId, PaypalModel.SecretId);
            }
            _client = new PayPalHttpClient(_environment);
        }

        public async Task<PaypalReturnModel> CreateOrder(string retUrl, string cancelUrl, string currency, decimal amount, string websiteName)
        {
            var result = new PaypalReturnModel("Error processing payment.");
            var orderRequest = new OrderRequest
            {
                CheckoutPaymentIntent = "CAPTURE",
                PurchaseUnits = new List<PurchaseUnitRequest>
            {
                new PurchaseUnitRequest
                {
                    AmountWithBreakdown = new AmountWithBreakdown
                    {
                        CurrencyCode = currency,
                        Value = amount.ToString("F2")
                    }
                }
            },
                ApplicationContext = new ApplicationContext
                {
                    UserAction = "PAY_NOW",
                    LandingPage = "NO_PREFERENCE",
                    ReturnUrl = retUrl,         //"https://localhost:44300/Payment/Success",
                    CancelUrl = cancelUrl,       //"https://localhost:44300/Payment/Cancel"
                    BrandName = websiteName,
                    ShippingPreference = "NO_SHIPPING"
                }
            };

            var request = new OrdersCreateRequest();
            request.Prefer("return=representation");
            request.RequestBody(orderRequest);

            var response = _client.Execute(request).Result;
            var res = response.Result<Order>();

            // Get approval link to redirect user to PayPal
            foreach (var link in res.Links)
            {
                if (link.Rel.Equals("approve", StringComparison.OrdinalIgnoreCase))
                {
                    result.Data = link.Href;
                    result.Success = true;
                    result.Message = "Order created successfully.";
                }
            }

            result.Token = res.Id;
            return result;
        }

        public async Task<bool> CaptureOrder(string orderId)
        {
            var request = new OrdersCaptureRequest(orderId);
            request.RequestBody(new OrderActionRequest());

            var response = await _client.Execute(request);
            var result = response.Result<Order>();

            return result.Status.Equals("COMPLETED", StringComparison.OrdinalIgnoreCase);
        }


    }
}
