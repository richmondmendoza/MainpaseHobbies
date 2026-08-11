using Dto.Enums;
using Repository.Repo.Order;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.App_Filters;
using Web.Controllers;

namespace Web.Areas.portal.Controllers
{
    public class BaseAdminController : BaseController
    {

        public JsonResult GetPendingOrderCount()
        {
            int count = 0;

            var status = string.Join("|", new List<int>()
            {
                (int)OrderStatusEnum.Pending,
                (int)OrderStatusEnum.Processing,
                (int)OrderStatusEnum.Refunded,
            });

            count = new OrderRepo().GetListAllByFilter(status).Count();
            return Json(count, JsonRequestBehavior.AllowGet);
        }
    }
}