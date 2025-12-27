using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.App_Filters;

namespace Web.Areas.portal.Controllers
{
    [PortalAuthorize]
    public class DashboardController : BaseAdminController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}