using Dto.Enums;
using Repository.Repo.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.App_Filters;

namespace Web.Areas.portal.Controllers
{
    [PortalAuthorize]
    public class OrderManagementController : BaseAdminController
    {
        OrderRepo _order = new OrderRepo();

        public ActionResult List_Pending()
        {
            var status = string.Join("|", new List<int>()
            {
                (int)OrderStatusEnum.Pending,
                (int)OrderStatusEnum.Processing,
                (int)OrderStatusEnum.Refunded,
            });
            return View("Index", _order.GetListAllByFilter(status));
        }
        public ActionResult List_Completed()
        {
            var status = string.Join("|", new List<int>()
            {
                (int)OrderStatusEnum.Completed
            });
            return View("Index", _order.GetListAllByFilter(status));
        }

        public ActionResult Details(int id)
        {
            var record = _order.Get(id);
            return View(record);
        }

        public ActionResult TaggedAsPaid(int id, string orderNumber)
        {
            var result = _order.Pay(id, orderNumber);

            ShowMessage(result.Message, result.Success);
            return RedirectToAction("Details", new { id = id });
        }

        public ActionResult ForDelivery(int id)
        {
            var result = _order.ForDelivery(id);

            ShowMessage(result.Message, result.Success);
            return RedirectToAction("Details", new { id = id });
        }

        public ActionResult OrderCompleted(int id)
        {
            var result = _order.Completed(id);

            ShowMessage(result.Message, result.Success);
            return RedirectToAction("Details", new { id = id });
        }

        public ActionResult Refunded(int id)
        {
            var result = _order.Refunded(id);

            ShowMessage(result.Message, result.Success);
            return RedirectToAction("Details", new { id = id });
        }

        public ActionResult Delivered(int id)
        {
            var result = _order.DeliveryCompleted(id);

            ShowMessage(result.Message, result.Success);
            return RedirectToAction("Details", new { id = id });
        }



    }
}