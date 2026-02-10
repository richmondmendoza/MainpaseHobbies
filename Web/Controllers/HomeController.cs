using Dto;
using Dto.Dto;
using Infrastructure;
using Newtonsoft.Json;
using Repository.Repo;
using Repository.Repo.Order;
using Repository.Repo.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Web.Models;

namespace Web.Controllers
{
    public class HomeController : MainSiteController
    {
        CartRepo _cart = new CartRepo();

        public HomeController()
        {

        }


        public ActionResult Index()
        {
            ViewBag.Inventories = new InventoryRepo().GetListRandom(11);
            ViewBag.Events = EventRepo.GetListFeatured();
            ViewBag.Banners = new BannerRepo().GetList();

            return View();
            //return RedirectToAction("Index", "Dashboard", new { area = "portal" });
        }

        public ActionResult Events()
        {
            return View(EventRepo.GetList());
        }

        public ActionResult Shop()
        {
            return RedirectToAction("Search");
        }

        public ActionResult ContactUs()
        {
            return View();
        }

        public ActionResult Cart()
        {
            IEnumerable<CartDetailsDto> cartItems = new List<CartDetailsDto>();
            cartItems = CartRepo.GetList(Identity?.Id ?? 0, BaseController.UserSessionKey);
            return View(cartItems);
        }

        public ActionResult Search()
        {
            return View(new SearchViewModel());
        }

        [HttpPost]
        public ActionResult Search(string param)
        {
            var model = new SearchViewModel
            {
                SearchTerm = param
            };

            return View(model);
        }

        public ActionResult CardDetails(string cardId)
        {
            var model = new CardDetailsViewModel();

            var currentUrl = (TempData["PreviousUrl"]?.ToString() ?? "") as string;
            model.Details = new InventoryRepo().GetById(cardId, true);
            if (model.Details == null)
            {
                ShowErrorMessage("Card not found.");

                if (!string.IsNullOrEmpty(currentUrl))
                    return Redirect(currentUrl);

                return RedirectToAction("Index");
            }

            model.ScryfallCard = GetScryfallCard(model.Details.ScryfallId);

            var conversionRate = ConversionInfo.Amount;
            var currentPrice = model.Details.Price / conversionRate;
            var isFoiled = model.Details.FoilType.ToLower() != "non-foil" & model.Details.FoilType.ToLower() != "normal";
            var scryfallPrice = Convert.ToDecimal(isFoiled ? (model.ScryfallCard.Prices?.UsdFoil ?? model.Details.Price.ToString()) : (model.ScryfallCard.Prices?.Usd ?? model.Details.Price.ToString()));
            if (currentPrice != scryfallPrice)
            {
                model.Details.Price = scryfallPrice * conversionRate;
                InventoryRepo.UpdatePrice(model.Details.Id, model.Details.Price);
            }

            return View(model);
        }

        private ScryfallCard GetScryfallCard(string scryfallId)
        {
            try
            {
                var card = new ScryfallCard();
                string cardUrl = $"https://api.scryfall.com/cards/{scryfallId}";
                using (HttpClient client = new HttpClient())
                {
                    var version = $"{new Random().Next()}.{new Random().Next()}";
                    client.DefaultRequestHeaders.UserAgent.ParseAdd($"MyApp/{version}");

                    var cardJson = client.GetStringAsync(cardUrl).Result;
                    card = JsonConvert.DeserializeObject<ScryfallCard>(cardJson);

                    if (card.CardFaces != null & card.CardFaces.Any())
                    {
                        for (int index = 0; index < card.CardFaces.Count; index++)
                        {
                            var imageBytes = client.GetByteArrayAsync(card.CardFaces[index].ImageUris.Png).Result;
                            card.CardFaces[index].ImageData = Convert.ToBase64String(imageBytes);
                        }
                    }
                }

                return card;
            }
            catch (Exception ex)
            {
                return new ScryfallCard();
            }
        }

        public ActionResult GetResults(string search, string colors, string foils, string rarities, string cardTypes)
        {
            var items = new InventoryRepo().GetSearchResult(searchParam: search, colors: colors, rarities: rarities, foilTypes: foils, cardTypes: cardTypes);
            return PartialView("_SearchResults", items);
        }

        [HttpPost]
        public JsonResult AddToCart(string cardId, int quantity)
        {
            int inventoryId = Convert.ToInt32(Fletcher.Decrypt(cardId));
            var item = new CartDto
            {
                InventoryId = inventoryId,
                Quantity = quantity,
                DateCreated = DateTime.Now,
                UserId = Identity?.Id ?? 0,
                UserSessionId = BaseController.UserSessionKey,
            };

            var result = _cart.Add(item);

            return Json(new { Success = result.Success, Message = result.Message }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult DeleteCart(ConfirmDto dto)
        {
            int inventoryId = Convert.ToInt32(Fletcher.Decrypt(dto.Param1));
            var result = _cart.Delete(inventoryId);

            ShowMessage(result.Message, result.Success);

            return RedirectToAction("cart");
        }

        [HttpPost]
        public ActionResult UpdateCartItem(string id, int quantity)
        {
            int inventoryId = Convert.ToInt32(Fletcher.Decrypt(id));
            var result = _cart.Update(inventoryId, quantity);

            if (!result.Success)
            {
                ShowErrorMessage(result.Message);
            }

            return RedirectToAction("cart");
        }

        public ActionResult TermsOfService()
        {
            return View();
        }

        public ActionResult PrivacyPolicy()
        {
            return View();
        }

        public ActionResult AboutUs()
        {
            return View();
        }


    }
}