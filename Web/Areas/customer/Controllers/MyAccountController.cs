using Dto.User;
using Newtonsoft.Json;
using Repository.Repo;
using Repository.Repo.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Web.App_Filters;
using Web.Models;

namespace Web.Areas.customer.Controllers
{
    [CustomerAuthorize]
    public class MyAccountController : BaseCustomerController
    {
        UserRepo _repo = new UserRepo();

        [AllowAnonymous]
        public ActionResult Login()
        {
            if (Identity != null)
            {
                return RedirectToAction("Index", "MyOrders", new { area = "customer" });
            }
            return View(new LoginViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login(LoginViewModel model)
        {
            if (string.IsNullOrEmpty(model.Username))
            {
                ShowErrorMessage("Enter a valid username.");
                return View(model);
            }

            if (string.IsNullOrEmpty(model.Password))
            {
                ShowErrorMessage("Please enter your password.");
                return View(model);
            }

            var result = _repo.Authenticate(model.Username, model.Password);

            if (result.Success)
            {
                var user = (AuthenticatedUserDto)result.Data ?? new AuthenticatedUserDto();
                _formsAuthenticationService.SetAuthCookie(user, model.RememberMe);
                AuditLogRepo.CreateLog("Login", user.Id, user.Username, "Users", JsonConvert.SerializeObject(user));
                return RedirectToAction("Index", "MyOrders", new { area = "customer" });
            }

            ShowErrorMessage(result.Message);
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View(new UserViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(UserViewModel model)
        {
            var result = _repo.Create(model.ToDto());

            if (result.Success)
            {
                var user = (AuthenticatedUserDto)result.Data ?? new AuthenticatedUserDto();
                _formsAuthenticationService.SetAuthCookie(user, true);
                AuditLogRepo.CreateLog("Register", user.Id, user.Username, "Users", JsonConvert.SerializeObject(user));
                return RedirectToAction("Index", "MyOrders", new { area = "customer" });
            }

            ShowErrorMessage(result.Message);
            return View(model);
        }

        public ActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }

        public ActionResult Logout()
        {
            _formsAuthenticationService.SignOut();
            return RedirectToAction("Index", "Home", new { area = "" });
        }

    }
}