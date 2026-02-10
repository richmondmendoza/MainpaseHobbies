using Dto;
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
    public class BannerController : BaseAdminController
    {
        BannerRepo _repo = new BannerRepo();

        public ActionResult List()
        {
            return View(_repo.GetList());
        }

        [HttpPost]
        public ActionResult Add(BannerViewModel model)
        {
            var result = _repo.Save(model.ToDto());
            ShowMessage(result.Message, result.Success);
            return RedirectToAction("list");
        }

        [HttpPost]
        public ActionResult Delete(ConfirmDto dto)
        {
            var result = _repo.Delete(Convert.ToInt32(dto.Param1));
            ShowMessage(result.Message, result.Success);
            return RedirectToAction("list");
        }
    }
}