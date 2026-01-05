using Dto;
using Dto.Dto;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using Repository.Repo;
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
    public class EventsController : BaseAdminController
    {
        EventRepo _event = new EventRepo();

        public ActionResult Index()
        {
            return View(EventRepo.GetList());
        }

        public ActionResult Add()
        {
            return View(new EventViewModel());
        }

        [HttpPost]
        public ActionResult Add(EventViewModel model)
        {
            var result = _event.Add(model.ToDto());
            var eventData = (EventDto)result.Data ?? new EventDto();

            if (result.Success)
            {
                var data = JsonConvert.SerializeObject(eventData);
                AuditLogRepo.CreateLog("Create Event", Identity.Id, Identity.Username, "Events", data);
            }

            ShowMessage(result.Message, result.Success);

            if (result.Success)
                return RedirectToAction("Update", new { id = eventData.Id });


            return View(model);
        }

        public ActionResult Update(int id)
        {
            return View(new EventViewModel(EventRepo.GetDetails(id)));
        }

        [HttpPost]
        public ActionResult Update(EventViewModel model)
        {
            var result = _event.Update(model.ToDto());

            if (result.Success)
            {
                var data = JsonConvert.SerializeObject(((EventDto)result.Data ?? new EventDto()));
                AuditLogRepo.CreateLog("Update Event", Identity.Id, Identity.Username, "Events", data);
            }

            ShowMessage(result.Message, result.Success);
            return RedirectToAction("update", new { id = model.Id });
        }

        [HttpPost]
        public ActionResult Delete(ConfirmDto dto)
        {
            var id = Convert.ToInt32(dto.Param1);
            var result = _event.Delete(id);

            if (result.Success)
            {
                var data = JsonConvert.SerializeObject(((EventDto)result.Data ?? new EventDto()));
                AuditLogRepo.CreateLog("Delete Event", Identity.Id, Identity.Username, "Events", data);
            }

            ShowMessage(result.Message, result.Success);

            if (result.Success)
                return RedirectToAction("index");

            return RedirectToAction("update", new { id = id });
        }

    }
}