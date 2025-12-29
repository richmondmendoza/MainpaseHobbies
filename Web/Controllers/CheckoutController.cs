using Dto.Dto;
using Dto.Enums;
using Dto.User;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Repository.Repo;
using Repository.Repo.Order;
using Repository.Repo.User;
using System;
using System.Collections.Generic;
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
                Price = a.Price,
                Quantity = a.Quantity,
                ProductName = a.ProductName,
                SubName = "",
                Description = "",
                Total = a.Price * a.Quantity,
                SerialNumber = "",
            }).ToList();

            model.SubTotal = model.Items.Sum(a => a.Total);
            model.Total = model.Shipping + model.SubTotal;

            return View(model);
        }

        [HttpPost]
        public ActionResult Index(CheckoutViewModel model)
        {
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

            var order = new OrderRepo().Add(model.ToDto());

            if (order.Success)
            {
                var payment = _payment.Add(new PaymentDto()
                {
                    Amount = model.Total + model.Shipping,
                    CreatedAt = DateTime.Now,
                    Currency = model.Currency,
                    CustomerDetailId = (int)customer.Data,
                    OrderId = (int)order.Data,
                    PaymentId = Guid.NewGuid().ToString(),
                    Status = PaymentStatus.Created,
                });
            }

            ShowMessage(order.Message, order.Success);

            return RedirectToAction("Index");
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
    }
}