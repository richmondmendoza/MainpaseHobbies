using Dto;
using Dto.Dto;
using Dto.Enums;
using Newtonsoft.Json;
using Repository.Repo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Web.App_Filters;
using Web.Models;

namespace Web.Areas.portal.Controllers
{
    [PortalAuthorize]
    public class InventoryController : BaseAdminController
    {
        InventoryRepo _repo = new InventoryRepo();
        byte[] _imageBytes = new byte[0];

        public ActionResult List()
        {
            var records = _repo.GetList();
            return View(records);
        }

        public ActionResult Add()
        {
            return View(new InventoryViewModel());
        }

        [HttpPost]
        public ActionResult Add(InventoryViewModel model)
        {
            model.CreatedBy = "";
            var result = _repo.Create(model.ToDto());

            ShowMessage(result.Message, result.Success);

            if (!result.Success)
                return View(model);

            return RedirectToAction("add");
        }

        public ActionResult Update(int id)
        {
            var record = _repo.GetDetailsById(id);
            return View(new InventoryViewModel(record));
        }

        [HttpPost]
        public ActionResult Update(InventoryViewModel model)
        {
            return RedirectToAction("update", new { id = model.Id });
        }

        public ActionResult Adjustments()
        {
            return View();
        }

        public ActionResult Adjust()
        {
            return View();
        }

        public ActionResult Upload()
        {
            var model = new List<InventoryDetailsDto>();
            return View(model);
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            var model = new List<InventoryDetailsDto>();

            using (var reader = new StreamReader(file.InputStream))
            {
                using (var csv = new CsvHelper.CsvReader(reader, System.Globalization.CultureInfo.InvariantCulture))
                {
                    csv.Context.RegisterClassMap<UploadDataMap>();
                    model = csv.GetRecords<UploadDataViewModel>().ToList().Select(a => new InventoryDetailsDto()
                    {
                        Name = a.Name,
                        SetCode = a.SetCode,
                        SetName = a.SetName,
                        Collector = a.CollectorNumber,
                        FoilType = a.Foil,
                        Rarity = a.Rarity,
                        ManaboxId = Convert.ToInt32(a.ManaBoxId ?? "0"),
                        ScryfallId = a.ScryfallId,
                        Price = a.PurchasePrice,
                        Misprint = a.Misprint,
                        Tampered = a.Altered,
                        Condition = a.Condition.ToUpper().Replace("_", " "),
                        Language = a.Language.ToLower(),
                        PurchaseCurrency = a.PurchasePriceCurrency.ToUpper(),
                        CreatedBy = $"Uploaded by .",
                        DateCreated = DateTime.Now,
                        InventoryCounts = new List<InventoryCountDto>()
                            {
                                new InventoryCountDto()
                                {
                                    Quantity = a.Quantity,
                                    CreatedBy = $"Uploaded by .",
                                    DateCreated = DateTime.Now,
                                    UOM = "PC",
                                    Remarks = $"Inventory uploaded by .",
                                    Type = InventoryCountTypeEnum.Upload,
                                }
                            }
                    }).ToList();
                }
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult SaveUploaded(IEnumerable<InventoryDetailsDto> list)
        {
            var result = _repo.CreateBulk(list);

            ShowMessage(result.Message, result.Success);

            return RedirectToAction("List");
        }



    }
}