using Dto;
using Dto.Dto;
using Infrastructure;
using Newtonsoft.Json;
using Repository.Repo;
using Repository.Repo.Order;
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
    public class HomeController : BaseController
    {
        CartRepo _cart = new CartRepo();

        public HomeController()
        {

        }


        public ActionResult Index()
        {
            ViewBag.Inventories = new InventoryRepo().GetListRandom(11);
            return View();
            //return RedirectToAction("Index", "Dashboard", new { area = "portal" });
        }

        public ActionResult Events()
        {
            return View();
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
            model.Details = new InventoryRepo().GetById(cardId);
            if (model.Details == null)
            {
                ShowErrorMessage("Card not found.");

                if (!string.IsNullOrEmpty(currentUrl))
                    return Redirect(currentUrl);

                return RedirectToAction("Index");
            }

            model.ScryfallCard = GetScryfallCard(model.Details.ScryfallId);
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
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("MyApp/1.0");

                    var cardJson = client.GetStringAsync(cardUrl).Result;
                    card = JsonConvert.DeserializeObject<ScryfallCard>(cardJson);
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
    }
}