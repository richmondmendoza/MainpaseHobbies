using Newtonsoft.Json;
using Repository.Repo;
using Repository.Repo.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.App_Filters;
using Web.Models;

namespace Web.Areas.portal.Controllers
{
    [PortalAuthorize]
    public class ConversionController : BaseAdminController
    {
        public ActionResult Index()
        {
            var item = ConversionRepo.Get();
            return View(new ConversionViewModel(item));
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Index(ConversionViewModel model)
        {
            var result = ConversionRepo.SaveConversion(model.ToDto());

            if (result.Success)
            {
                AuditLogRepo.CreateLog("Update", Identity.Id, Identity.Username, "Conversion", JsonConvert.SerializeObject(result.Data));
            }

            ShowMessage(result.Message, result.Success);
            return RedirectToAction("index");
        }
    }
}