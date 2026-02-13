using Repository.Repo.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.App_Filters;

namespace Web.Areas.portal.Controllers
{
    [PortalAuthorize]
    public class FoilTypeController : BaseAdminController
    {
        FoilTypeRepo _repo = new FoilTypeRepo();

        public ActionResult List()
        {
            return View(_repo.GetList());
        }

        [HttpPost]
        public ActionResult Add(string name)
        {
            var result = _repo.Add(name);
            ShowMessage(result.Message, result.Success);
            return RedirectToAction("List");
        }
    }
}