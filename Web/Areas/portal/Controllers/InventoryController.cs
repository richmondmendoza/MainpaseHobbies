using Dto;
using Dto.Dto;
using Dto.Enums;
using Newtonsoft.Json;
using Repository.Repo;
using Repository.Repo.User;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Net.Http;
using System.Text.RegularExpressions;
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
            var filters = _repo.GetFilters();

            ViewBag.SetCodes = filters.Item1.Select(a => new SelectListItem() { Value = a, Text = a }).ToList();
            ViewBag.Categories = filters.Item2.Select(a => new SelectListItem() { Value = a, Text = a }).ToList();
            ViewBag.CardOwners = filters.Item3.Select(a => new SelectListItem() { Value = a.Item1.ToString(), Text = a.Item2 }).ToList();
            var records = _repo.GetList("grand archive|magic the gathering", isPHPDisplay: true);
            return View(records);
        }

        public ActionResult LoadList()
        {
            var requestData = new DataTableRequestData();

            var collectionGroup = requestData.GetCustomQuery("collectionGroup");
            var cardOwnerId = Convert.ToInt32(requestData.GetCustomQuery("cardOwnerId"));
            var foilType = requestData.GetCustomQuery("foilType");
            var category = requestData.GetCustomQuery("category");
            var searchParam = requestData.GetCustomQuery("searchParam");

            var model = _repo.GetList(collectionGroup, cardOwnerId, "", category, searchParam, true, 0, foilType);

            var sortPattern = "name_desc";
            try
            {
                sortPattern = requestData?.SortPattern ?? "name_desc";
            }
            catch { }
            string sort = sortPattern.Replace("_", " ");

            if (model.Count() > 0)
                model = model.OrderBy(sort);

            var recordsTotal = model.Count();

            var data = (requestData.PageSize <= 0) ? model.ToList() :
                model.Skip(requestData.Skip).Take(requestData.PageSize).ToList();

            var jResult = Json(new { draw = requestData.Draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);

            jResult.MaxJsonLength = int.MaxValue;

            return jResult;
        }

        public ActionResult Add(string uID = "", string collectionGroup = "")
        {
            var model = new InventoryViewModel();

            if (!string.IsNullOrEmpty(uID))
            {
                switch (collectionGroup)
                {
                    case "Magic the Gathering":
                        var card_mtg = _repo.FetchCardDetailsAsync_Scryfall(uID).Result;

                        if (card_mtg.Item2 != null)
                        {
                            model.CollectionGroup = collectionGroup;
                            model.Name = card_mtg.Item2.CardFaces.Any() ? card_mtg.Item2.CardFaces.FirstOrDefault()?.Name : card_mtg.Item2.Name;
                            model.SetCode = card_mtg.Item2.Set;
                            model.SetName = card_mtg.Item2.SetName;
                            model.ScryfallId = uID;
                            model.Rarity = card_mtg.Item2.Rarity;
                            model.FoilType = card_mtg.Item2.Foil ? "Foil" : "Non-Foil";

                            model.Collector = card_mtg.Item2.CollectorNumber;
                            model.Language = card_mtg.Item2.Lang;
                            model.PurchaseCurrency = "PHP";
                            model.Color = _repo.GetColorIdentityString(card_mtg.Item2.ColorIdentity);
                            model.ManaCost = card_mtg.Item2.ManaCost;
                            model.CardType = card_mtg.Item2.TypeLine;
                            model.IllustratedBy = card_mtg.Item2.Artist;
                            model.Description = card_mtg.Item2.OracleText;
                            model.Image = card_mtg.Item1;

                            model.Price = Convert.ToDecimal(card_mtg.Item2.Prices.Usd ?? "0") * (ConversionInfo?.Amount ?? 0);
                        }
                        break;
                    case "Grand Archive":
                        var cardNameAfterSlash = uID.Contains("-") ? Regex.Replace(uID, "-.*$", "") : uID;
                        var slugName = cardNameAfterSlash.ToLower().Replace(" ", "-").Replace(",", "").Replace("'", "").Replace(":", "").Replace("ä", "a").Replace("ö", "o").Replace("ü", "u").Replace("ß", "ss");
                        var card_ga = _repo.FetchCardDetailsAsync_GrandArchive(slugName).GetAwaiter().GetResult();

                        if (card_ga.Item2 != null)
                        {
                            model.CollectionGroup = collectionGroup;
                            model.Name = card_ga.Item2.name;
                            model.SetCode = card_ga.Item2.editions.FirstOrDefault()?.set?.prefix ?? "";
                            model.SetName = card_ga.Item2.editions.FirstOrDefault()?.set?.name ?? "";
                            model.ScryfallId = !string.IsNullOrEmpty(card_ga.Item2.slug ?? "") ? card_ga.Item2.slug : slugName;
                            model.Rarity = card_ga.Item2.editions.FirstOrDefault()?.rarity.ToString() ?? "";
                            model.FoilType = (card_ga.Item2.editions.FirstOrDefault()?.circulationTemplates.FirstOrDefault()?.foil ?? false) ? "Foil" : "Non-Foil";

                            model.Collector = card_ga.Item2.editions.FirstOrDefault()?.collector_number.ToString() ?? "";
                            model.Language = card_ga.Item2.editions.FirstOrDefault()?.set?.language ?? "en";
                            model.PurchaseCurrency = "PHP";
                            model.ManaCost = $"{card_ga.Item2.cost.type}|{card_ga.Item2.cost.value}";
                            model.Color = string.Join(", ", card_ga.Item2.elements).TrimEnd();
                            model.CardType = string.Join(", ", card_ga.Item2.classes).TrimEnd();
                            model.IllustratedBy = card_ga.Item2.editions.FirstOrDefault()?.illustrator ?? "";
                            model.Description = card_ga.Item2.effect ?? "";
                            model.Image = card_ga.Item1;

                            model.Price = 0;
                        }
                        break;
                }
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Add(InventoryViewModel model)
        {
            model.CreatedBy = "";
            var result = _repo.Create(model.ToDto());
            if (result.Success)
            {
                AuditLogRepo.CreateLog("New Inventory", Identity.Id, Identity.Username, "Inventory", JsonConvert.SerializeObject(result.Data));
            }

            ShowMessage(result.Message, result.Success);

            if (!result.Success)
                return View(model);

            return RedirectToAction("add");
        }

        public ActionResult Update(int id)
        {
            var record = _repo.GetDetailsById(id);
            //record.Price = record.Price / ConversionInfo.Amount;
            return View(new InventoryViewModel(record));
        }

        [HttpPost]
        public ActionResult Update(InventoryViewModel model)
        {
            //model.Price = model.Price * ConversionInfo.Amount;
            var result = _repo.Update(model.ToDto());

            if (result.Success)
            {
                AuditLogRepo.CreateLog("Update Inventory", Identity.Id, Identity.Username, "Inventory", JsonConvert.SerializeObject(result.Data));
            }

            ShowMessage(result.Message, result.Success);
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

            var names = new UserRepo().GetNames().ToList().Select(a => new SelectListItem() { Value = a.Item1.ToString(), Text = a.Item2 });
            ViewBag.CardOwners = names;
            TempData["OwnerId"] = 0;
            TempData["Group"] = "";

            return View(model);
        }

        public ActionResult Bulk()
        {
            var filters = _repo.GetFilters();

            ViewBag.SetCodes = filters.Item1.Select(a => new SelectListItem() { Value = a, Text = a }).ToList();
            ViewBag.Categories = filters.Item2.Select(a => new SelectListItem() { Value = a, Text = a }).ToList();
            ViewBag.CardOwners = filters.Item3.Select(a => new SelectListItem() { Value = a.Item1.ToString(), Text = a.Item2 }).ToList();

            return View();
        }

        public ActionResult LoadBulkList()
        {
            var requestData = new DataTableRequestData();

            var collectionGroup = requestData.GetCustomQuery("collectionGroup");
            var cardOwnerId = Convert.ToInt32(requestData.GetCustomQuery("cardOwnerId"));
            var setCode = requestData.GetCustomQuery("setCode");
            var category = requestData.GetCustomQuery("category");
            var searchParam = requestData.GetCustomQuery("searchParam");

            var model = _repo.GetList(collectionGroup, cardOwnerId, setCode, category, searchParam, true, Identity.HasProtalAdminAccess ? 0 : Identity.Id);

            var sortPattern = "name_desc";
            try
            {
                sortPattern = requestData?.SortPattern ?? "name_desc";
            }
            catch { }
            string sort = sortPattern.Replace("_", " ");

            if (model.Count() > 0)
                model = model.OrderBy(sort);

            var recordsTotal = model.Count();

            var data = (requestData.PageSize <= 0) ? model.ToList() :
                model.Skip(requestData.Skip).Take(requestData.PageSize).ToList();

            var jResult = Json(new { draw = requestData.Draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data }, JsonRequestBehavior.AllowGet);

            jResult.MaxJsonLength = int.MaxValue;

            return jResult;
        }

        [HttpPost]
        public ActionResult SaveBulk(string action, string ids, int ownerId)
        {
            var result = _repo.BulkUpdate(action, ids, ownerId);
            return Json(new { Success = result.Success, Message = result.Message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Upload(HttpPostedFileBase file, int ownerId, string collectionGroup)
        {
            var model = new List<InventoryDetailsDto>();

            ViewBag.CardOwners = new UserRepo().GetNames().ToList().Select(a => new SelectListItem() { Value = a.Item1.ToString(), Text = a.Item2 });
            TempData["OwnerId"] = ownerId;
            TempData["Group"] = collectionGroup;

            if (collectionGroup == "Magic the Gathering")
                model = UploadMTG(file, ownerId, collectionGroup).ToList();
            else if (collectionGroup == "Grand Archive")
                model = UploadGA(file, ownerId, collectionGroup).ToList();

            return View(model);
        }

        [HttpPost]
        public ActionResult SaveUploaded(IEnumerable<InventoryDetailsDto> list)
        {
            var result = _repo.CreateBulk(list);

            ShowMessage(result.Message, result.Success);

            return RedirectToAction("List");
        }

        [HttpPost]
        public ActionResult UpdateCount(InventoryCountDto dto)
        {
            dto.CreatedBy = $"Count updated by [{Identity.Username}].";
            dto.DateCreated = DateTime.Now;
            dto.UOM = "PC";
            dto.Type = InventoryCountTypeEnum.Upload;
            dto.IsDeleted = false;

            var result = new InventoryCountRepo().Create(dto);

            if (result.Success)
            {
                AuditLogRepo.CreateLog("Update Inventory Count", Identity.Id, Identity.Username, "InventoryCount", JsonConvert.SerializeObject(dto));
            }

            ShowMessage(result.Message, result.Success);
            return RedirectToAction("Update", new { id = dto.InventoryId });
        }

        private IEnumerable<InventoryDetailsDto> UploadMTG(HttpPostedFileBase file, int ownerId, string collectionGroup)
        {
            var model = new List<InventoryDetailsDto>();
            var conversionRate = ConversionInfo.Amount;

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
                        Price = a.PurchasePrice * conversionRate,
                        Misprint = a.Misprint,
                        Tampered = a.Altered,
                        Condition = a.Condition.ToUpper().Replace("_", " "),
                        Language = a.Language.ToLower(),
                        PurchaseCurrency = a.PurchasePriceCurrency.ToUpper(),
                        CreatedBy = $"Uploaded by [{Identity.Username}].",
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
                            },
                        OwnerId = ownerId,
                        CollectionGroup = collectionGroup,
                    }).ToList();
                }
            }

            return model;
        }

        private IEnumerable<InventoryDetailsDto> UploadGA(HttpPostedFileBase file, int ownerId, string collectionGroup)
        {
            var model = new List<InventoryDetailsDto>();
            var conversionRate = ConversionInfo.Amount;

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
                        Price = a.PurchasePrice * conversionRate,
                        Misprint = a.Misprint,
                        Tampered = a.Altered,
                        Condition = a.Condition.ToUpper().Replace("_", " "),
                        Language = a.Language.ToLower(),
                        PurchaseCurrency = a.PurchasePriceCurrency.ToUpper(),
                        CreatedBy = $"Uploaded by [{Identity.Username}].",
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
                            },
                        OwnerId = ownerId,
                        CollectionGroup = collectionGroup,
                    }).ToList();
                }
            }

            return model;
        }

    }
}