using Dto;
using Dto.Dto;
using Dto.Enums;
using Dto.User;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using PaymentGateway.Coins;
using PaymentGateway.Coins.Services;
using Paypal.Interfaces;
using Repository.Repo;
using Repository.Repo.Order;
using Repository.Repo.User;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers
{
    public class CheckoutController : MainSiteController
    {
        PaymentRepo _payment = new PaymentRepo();
        CustomerRepo _customer = new CustomerRepo();

        public ActionResult Index()
        {
            var items = CartRepo.GetList(Identity?.Id ?? 0, UserSessionKey);
            var model = new CheckoutViewModel()
            {
                UserId = Identity?.Id ?? 0,
                DateCreated = DateTime.Now,
                PaymentMethod = PaymentMethodEnum.Cash,
                Status = OrderStatusEnum.Pending,
                DeliveryStatus = DeliveryStatusEnum.Pending,
                DeliveryMethod = DeliveryMethodEnum.StorePickup,
                Currency = items.FirstOrDefault()?.Currency ?? "USD",
            };

            if ((Identity?.Id ?? 0) > 0)
            {
                var customer = _customer.GetByUserId(Identity.Id);
                if (customer != null)
                {
                    model.Address1 = customer.Address1;
                    model.Address2 = customer.Address2;
                    model.CustomerName = $"{customer.Firstname} {customer.Lastname}";
                    model.ContactEmail = customer.Email;
                    model.ContactNumber = customer.Mobile;
                }
            }

            model.Items = items.Select(a => new OrderItemDto()
            {
                Id = a.Id,
                Price = a.Price,
                Quantity = a.Quantity,
                ProductName = a.ProductName,
                SubName = "",
                Description = "",
                Total = a.Price * a.Quantity,
                SerialNumber = "",
                PreviousOwnerName = a.PreviousOwnerName
            }).ToList();

            model.SubTotal = model.Items.Sum(a => a.Total);
            model.Total = model.Shipping + model.SubTotal;

            if (!(model.Items.Count > 0))
                return Redirect(Url.Action("Index", "Home", new { area = "" }));

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(CheckoutViewModel model)
        {
            var order = new ReturnValue();
            var requestId = "";
            var exisitingOrderNumber = Session["OrderNumber"] as string;

            if (!string.IsNullOrEmpty(exisitingOrderNumber))
            {
                requestId = exisitingOrderNumber;
            }
            else
            {
                requestId = $"ORD{DateTime.UtcNow:yyyyMMddHHmmssfff}".Substring(0, 19);
                Session["OrderNumber"] = requestId;
            }
            model.OrderNumber = requestId;

            var customer = _customer.Add(new CustomerDetailDto()
            {
                Address1 = model.Address1,
                Address2 = model.Address2,
                ShippingAddress1 = model.Address1,
                ShippingAddress2 = model.Address2,
                DateCreated = DateTime.Now,
                Email = model.ContactEmail,
                Mobile = model.ContactNumber,
                Firstname = model.CustomerName,
                Lastname = "",
                UserId = Identity?.Id ?? 0,
            });


            ReturnValue payment;
            switch (model.PaymentMethod)
            {
                case PaymentMethodEnum.Cash:

                    model.OrderNumber = requestId;
                    order = new OrderRepo().Add(model.ToDto());

                    payment = _payment.Add(new PaymentDto()
                    {
                        Amount = model.Total + model.Shipping + model.Tax,
                        CreatedAt = DateTime.Now,
                        Currency = model.Currency,
                        CustomerDetailId = (int)customer.Data,
                        OrderId = (int)order.Data,
                        PaymentId = PaymentMethodEnum.Cash.ToString(),
                        Status = PaymentStatus.Created,
                        PayoneerId = requestId,
                    });
                    return RedirectToAction("Result", new { val = Fletcher.Encrypt(((int)order.Data).ToString()) });

                case PaymentMethodEnum.CreditCard:
                case PaymentMethodEnum.PayPal:
                case PaymentMethodEnum.BankTransfer:
                    return Content("Error creating payment.");
                case PaymentMethodEnum.CoinsPH:

                    var redirectUrls = new RedirectUrls()
                    {
                        success = Url.Action("CoinsSuccess", "Checkout", new { requestId = requestId }, Request.Url.Scheme),
                        cancel = Url.Action("CoinsCancel", "Checkout", new { requestId = requestId }, Request.Url.Scheme),
                        failure = Url.Action("CoinsFailure", "Checkout", new { requestId = requestId }, Request.Url.Scheme),
                        defaultUrl = Url.Action("CoinsReturn", "Checkout", new { requestId = requestId }, Request.Url.Scheme),
                    };

                    var webhookUrl = Url.Action("CoinsWebhook", "Webhook", null, Request.Url.Scheme);
                    var orders = model.Items.Select(a => new ProductDetails()
                    {
                        amount = (a.Total).ToString("F2"),
                        desc = a.Description,
                        name = a.ProductName,
                        quantity = a.Quantity.ToString(),
                        type = "product",
                    }).ToList();
                    orders.Add(new ProductDetails()
                    {
                        amount = model.Shipping.ToString("F2"),
                        desc = "Shipping Fee",
                        name = "Shipping",
                        quantity = "1",
                        type = "fee",
                    });
                    orders.Add(new ProductDetails()
                    {
                        amount = model.Tax.ToString("F2"),
                        desc = "TAX Fee",
                        name = "Tax",
                        quantity = "1",
                        type = "fee",
                    });

                    var fee = 0.00m;
                    //var coinRequest = new CoinsCreateCheckoutRequest()
                    //{
                    //    amount = (model.Total + model.Shipping + model.Tax).ToString("F2"),
                    //    currency = "PHP",
                    //    expireSeconds = "600",
                    //    feeAmount = fee.ToString("F2"),
                    //    merchantName = SystemInfo.LongName,
                    //    productDetails = orders,
                    //    redirectUrl = redirectUrls,
                    //    requestId = requestId,
                    //    totalAmount = (model.Total + model.Shipping + model.Tax + fee).ToString("F2"),
                    //    remark = $"Order #{requestId}",
                    //};
                    model.Total = 1;
                    var coinRequest = new
                    {
                        requestId = requestId,
                        type = "DYNAMIC",
                        source = SystemInfo.Name,
                        amount = (model.Total + model.Shipping + model.Tax).ToString("F2"),
                        currency = "PHP",
                        remark = $"Order #{requestId}",
                        expiredSeconds = 900
                    };

                    ViewBag.Amount = coinRequest.amount;

                    //        {
                    //            requestId = requestId,
                    //            amount = ,
                    //            currency = "PHP",
                    //            expireSeconds = "600",
                    //            merchantName = SystemInfo.LongName,
                    //            remark = $"Order #{requestId}",
                    //            description = $"Payment for Order #{requestId}",
                    //        }
                    //;

                    var existingQR = Session[$"CoinsPH_QR_{requestId}"] as QrModel;

                    if (existingQR != null && existingQR.Expiry > DateTime.Now)
                    {
                        return View("ShowQR", existingQR);
                    }

                    var qr = new CoinsPH().GenerateDynamicQR(coinRequest);
                    Session[$"CoinsPH_QR_{requestId}"] = qr;
                    Session["PaymentStatus"] = "PENDING";

                    //var coinResult = new CoinsPH().CreatePayment(coinRequest);

                    //if (coinResult?.status == 0 && coinResult.data?.checkoutUrl != null)
                    //{
                    //    order = new OrderRepo().Add(model.ToDto());
                    //    payment = _payment.Add(new PaymentDto()
                    //    {
                    //        Amount = model.Total + model.Shipping,
                    //        CreatedAt = DateTime.Now,
                    //        Currency = model.Currency,
                    //        CustomerDetailId = (int)customer.Data,
                    //        OrderId = (int)order.Data,
                    //        PaymentId = PaymentMethodEnum.CoinsPH.ToString(),
                    //        Status = PaymentStatus.Pending,
                    //        PayoneerId = requestId,
                    //    });

                    //    return Redirect(coinResult.data.checkoutUrl);
                    //}

                    //return Content("Error creating CoinsPH Order");

                    return View("ShowQR", qr);

                default:
                    break;
            }

            //ShowMessage(order.Message, order.Success);
            return View("Index", model);
        }

        [HttpPost]
        public ActionResult CheckoutLogin(CheckoutLogin model)
        {
            var userSessionKey = UserSessionKey;

            if (model.IsGuest)
            {
                _customer.Add(Identity?.Id ?? 0, model.Email);
            }
            else
            {
                var result = new UserRepo().Authenticate(model.Username, model.Password);

                if (result.Success)
                {
                    var user = (AuthenticatedUserDto)result.Data ?? new AuthenticatedUserDto();
                    _formsAuthenticationService.SetAuthCookie(user, false);
                    new CartRepo().UpdateOnLogin(user?.Id ?? 0, userSessionKey);
                    AuditLogRepo.CreateLog("Login", user.Id, user.Username, "Users", JsonConvert.SerializeObject(user));
                }
            }

            return RedirectToAction("index");
        }

        public ActionResult Result(string val = "")
        {
            if (!string.IsNullOrEmpty(val))
            {
                var res = "";
                try
                {
                    res = Fletcher.Decrypt(val);

                    if (res == "success")
                    {
                        ViewBag.Message = "Your order has been placed successfully.";
                    }
                    else
                    {
                        ViewBag.Message = "There was an issue with your order.";

                        if (int.TryParse(res, out int result))
                        {
                            var order = new OrderRepo().Get(Convert.ToInt32(res));

                            if (order != null)
                            {
                                Session["Total"] = $"{order.Currency.ToUpper()} {order.Total.ToString("n2")}";
                                Session["OrderNumber"] = order.OrderNumber;
                            }
                        }
                    }
                }
                catch { }

            }


            return View();
        }

        [HttpPost]
        public ActionResult CoinCallback(string requestId)
        {
            var body = new StreamReader(Request.InputStream).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(body);

            var orderId = data.merchant_order_id;
            var status = data.status;

            if (status == "paid")
            {
                // 1. Verify authenticity (signature/header if provided)
                // 2. Update order in DB

            }

            return new HttpStatusCodeResult(200);
        }

        [HttpGet]
        public async Task<ActionResult> CoinStatus(string checkoutId = null, string requestId = null)
        {
            var res = new CoinsPH().GetCheckoutStatusAsync(checkoutId, requestId);
            return Json(res, JsonRequestBehavior.AllowGet);
        }

        // These are the redirect landing pages configured in redirectUrl
        [HttpGet]
        public ActionResult CoinsSuccess(string requestId)
        {
            var order = new OrderRepo();
            var record = order.GetByOrderNumber(requestId);
            var payment = order.Pay(record.Id, record.OrderNumber);

            return View();
        }

        [HttpGet]
        public ActionResult CoinsFailure(string requestId)
        {
            return View();
        }

        [HttpGet]
        public ActionResult CoinsCancel(string requestId)
        {
            return View();
        }

        [HttpGet]
        public ActionResult CoinsReturn(string requestId)
        {
            var res = new CoinsPH().GetCheckoutStatusAsync("", requestId);
            if (res != null && res.status == 0)
            {
                var status = res.data?.status ?? "";
                ViewBag.LatestStatus = status;
            }
            return View();
        }

        public ActionResult CheckQrStatus()
        {
            string body;

            using (var reader = new StreamReader(Request.InputStream))
            {
                body = reader.ReadToEnd();
            }

            try
            {
                dynamic data = JsonConvert.DeserializeObject(body);

                var status = (string)data.data.status;

                if (status == "SUCCESS")
                {
                    Session["PaymentStatus"] = "PAID";
                }
            }
            catch (Exception ex) { }

            return new HttpStatusCodeResult(200);
        }


    }
}