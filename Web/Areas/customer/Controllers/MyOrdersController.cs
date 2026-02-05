using Dto;
using Dto.Dto;
using Dto.Enums;
using Dto.User;
using Infrastructure;
using Newtonsoft.Json;
using PaymentGateway.Coins;
using PaymentGateway.Coins.Services;
using Repository.Repo;
using Repository.Repo.Order;
using Repository.Repo.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.App_Filters;
using Web.Models;

namespace Web.Areas.customer.Controllers
{
    [CustomerAuthorize]
    public class MyOrdersController : BaseCustomerController
    {
        OrderRepo _order = new OrderRepo();
        PaymentRepo _payment = new PaymentRepo();

        public ActionResult Index()
        {
            var status = string.Join("|", new List<int>()
            {
                (int)OrderStatusEnum.Pending,
                (int)OrderStatusEnum.Processing,
                (int)OrderStatusEnum.Completed,
                (int)OrderStatusEnum.Refunded,
                (int)OrderStatusEnum.Cancelled,
            });
            return View(_order.GetListByUser(Identity.Id, status));
        }

        public ActionResult Details(int id)
        {
            var order = _order.Get(id);

            if (order == null)
            {
                ShowErrorMessage("Order not found");
                return RedirectToAction("Index");
            }

            return View(order);
        }

        public ActionResult Cancel(ConfirmDto dto)
        {
            var id = Convert.ToInt32(dto.Param1);
            var result = _order.Cancel(id);
            if (result.Success)
            {
                AuditLogRepo.CreateLog("Cancel Order", Identity.Id, Identity.Username, "Orders", JsonConvert.SerializeObject(result.Data));
            }

            ShowMessage(result.Message, result.Success);
            return RedirectToAction("Details", new { id = id });
        }

        public ActionResult PayWithCoins(int id)
        {
            var order = _order.Get(id);

            var redirectUrls = new RedirectUrls()
            {
                success = Url.Action("CoinsSuccess", "Checkout", new { area = "", requestId = order.OrderNumber }, Request.Url.Scheme),
                cancel = Url.Action("CoinsCancel", "Checkout", new { area = "", requestId = order.OrderNumber }, Request.Url.Scheme),
                failure = Url.Action("CoinsFailure", "Checkout", new { area = "", requestId = order.OrderNumber }, Request.Url.Scheme),
                defaultUrl = Url.Action("CoinsReturn", "Checkout", new { area = "", requestId = order.OrderNumber }, Request.Url.Scheme),
            };

            var webhookUrl = Url.Action("CoinsWebhook", "Webhook", new { area = "" }, Request.Url.Scheme);
            var orders = order.Items.Select(a => new ProductDetails()
            {
                //amount = (a.Total).ToString("F2"),
                amount = "1.00", //testing only
                desc = a.Description,
                name = a.ProductName,
                quantity = a.Quantity.ToString(),
                type = "product",
            }).ToList();
            orders.Add(new ProductDetails()
            {
                amount = order.Shipping.ToString("F2"),
                desc = "Shipping Fee",
                name = "Shipping",
                quantity = "1",
                type = "fee",
            });
            orders.Add(new ProductDetails()
            {
                amount = order.Tax.ToString("F2"),
                desc = "TAX Fee",
                name = "Tax",
                quantity = "1",
                type = "fee",
            });

            var fee = 0.00m;
            order.Total = 1;
            var coinRequest = new CoinsCreateCheckoutRequest()
            {
                amount = (order.Total + order.Shipping + order.Tax).ToString("F2"),
                currency = "PHP",
                expireSeconds = "600",
                feeAmount = fee.ToString("F2"),
                merchantName = SystemInfo.LongName,
                productDetails = orders,
                redirectUrl = redirectUrls,
                requestId = order.OrderNumber,
                totalAmount = (order.Total + order.Shipping + order.Tax + fee).ToString("F2"),
                remark = $"Order #{order.OrderNumber}",
            };

            var coinResult = new CoinsPH().CreatePayment(coinRequest);

            if (coinResult?.status == 0 && coinResult.data?.checkoutUrl != null)
            {
                var payment = _payment.GetPaymentByOrderId(order.Id, order.OrderNumber);
                if (payment != null)
                {
                    payment.PaymentId = PaymentMethodEnum.CoinsPH.ToString();
                    _payment.Update(payment);
                }
                else
                {
                    var customer = new CustomerRepo().GetByUserId(Identity.Id);
                    _payment.Add(new PaymentDto()
                    {
                        Amount = order.Total + order.Shipping,
                        CreatedAt = DateTime.Now,
                        Currency = "PHP",
                        CustomerDetailId = customer?.Id ?? 0,
                        OrderId = order.Id,
                        PaymentId = PaymentMethodEnum.CoinsPH.ToString(),
                        Status = PaymentStatus.Created,
                        PayoneerId = order.OrderNumber,
                    });
                }
                return Redirect(coinResult.data.checkoutUrl);
            }

            return RedirectToAction("Details", new { id = id });
        }

    }
}
