using Newtonsoft.Json;
using Repository.Repo;
using Repository.Repo.Order;
using Repository.Repo.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.App_Filters;

namespace Web.Areas.customer.Controllers
{
    [CustomerAuthorize]
    public class MyOrdersController : BaseCustomerController
    {
        OrderRepo _order = new OrderRepo();

        public ActionResult Index()
        {
            return View(_order.GetList());
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

        public ActionResult Cancel(int id)
        {
            var result = _order.Cancel(id);
            if(result.Success)
            {
                AuditLogRepo.CreateLog("Cancel Order", Identity.Id, Identity.Username, "Orders", JsonConvert.SerializeObject(result.Data));
            }

            ShowMessage(result.Message, result.Success);
            return RedirectToAction("Details", new { id = id });
        }



    }
}
