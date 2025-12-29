using Dto.User;
using Microsoft.Ajax.Utilities;
using Repository.Repo.Order;
using Repository.Repo.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Controllers
{
    public class MainSiteController : BaseController
    {
        internal static CustomerRepo _customer = new CustomerRepo();

        public static CustomerDetailDto GetCustomerDetail(string email)
        {
            var customer = _customer.GetDetail(email);

            if (customer == null)
            {
                return new CustomerDetailDto();
            }

            return customer;
        }

        public static void Subscribe(string email)
        {
            _customer.SubscribeToNewsLetter(email);
        }

        public JsonResult GetCartItemCount()
        {
            var cartRepo = new CartRepo();
            var count = CartRepo.GetCount(Identity?.Id ?? 0, UserSessionKey);

            return Json(new { Count = count }, JsonRequestBehavior.AllowGet);
        }

    }
}