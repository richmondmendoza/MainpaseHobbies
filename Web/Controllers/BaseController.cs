using Dto.BaseSettings;
using Dto.User;
using Repository.Repo;
using Repository.Repo.Order;
using Repository.Repo.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Web.App_Filters.Services;
using Web.App_Interfaces;

namespace Web.Controllers
{
    public class BaseController : Controller
    {
        public static SystemInfoDto SystemInfo;
        public static string UserSessionKey;
        internal IFormsAuthenticationService _formsAuthenticationService = new FormsAuthenticationServices();


        public BaseController()
        {
            if (string.IsNullOrEmpty(UserSessionKey))
                _formsAuthenticationService.SetUserSessionKey();

            if (SystemInfo == null)
            {
                var info = SystemInfoRepo.GetSystemInfo();
                if (info != null)
                {
                    SystemInfo = info;
                }
            }

            try
            {
                new CartRepo().DeleteTemporaryCarts();
            }
            catch (Exception ex) { }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var previousUrl = (TempData["PreviousUrl"]?.ToString() ?? "") as string;
            var currentUrl = (TempData["CurrentUrl"]?.ToString() ?? "") as string;

            if (Request?.UrlReferrer != null)
            {
                if (currentUrl != Request.UrlReferrer.ToString())
                {
                    if (previousUrl != Request.UrlReferrer.ToString())
                    {
                        if (!string.IsNullOrEmpty(currentUrl))
                            previousUrl = currentUrl;

                        currentUrl = Request.UrlReferrer.ToString();
                    }
                }
            }

            TempData["PreviousUrl"] = previousUrl;
            TempData["CurrentUrl"] = currentUrl;

            base.OnActionExecuting(filterContext);
        }

        public ActionResult GoBack(string ReturnUrl)
        {
            var previousUrl = (TempData["PreviousUrl"]?.ToString() ?? "") as string;
            if (!string.IsNullOrEmpty(previousUrl))
            {
                return Redirect(previousUrl);
            }
            return Redirect(ReturnUrl);
        }

        public void ShowMessage(string message)
        {
            ShowMessage(message, true);
        }

        public void ShowErrorMessage(string message)
        {
            ShowMessage(message, false);
        }

        public void ShowMessage(string message, bool isSuccess)
        {
            TempData["ViewMessage"] = message;
            TempData["ViewMessageType"] = isSuccess ? "Success" : "Error";
        }


        public static AuthenticatedUserDto Identity
        {
            get
            {
                IFormsAuthenticationService formsAuthenticationService = new FormsAuthenticationServices();
                return formsAuthenticationService.Identity();
            }
        }

    }
}