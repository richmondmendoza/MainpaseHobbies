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
        public ActionResult Index()
        {
            return View();
        }
    }
}