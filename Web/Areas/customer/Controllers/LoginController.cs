using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.Controllers;

namespace Web.Areas.customer.Controllers
{
    public class LoginController : BaseController
    {
        // GET: customer/login
        public ActionResult Index()
        {
            return View();
        }
    }
}