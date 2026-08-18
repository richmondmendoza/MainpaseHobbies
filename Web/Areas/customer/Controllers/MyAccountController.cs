using Dto.Enums;
using Dto.User;
using Newtonsoft.Json;
using Repository.Repo;
using Repository.Repo.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
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
            return View(new UserViewModel()
            {
                Role = UserRoleEnum.Customer.ToString()
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Register(UserViewModel model)
        {
            if (string.IsNullOrEmpty(model.Firstname) ||
                string.IsNullOrEmpty(model.LastName) ||
                string.IsNullOrEmpty(model.Username) ||
                string.IsNullOrEmpty(model.Password) ||
                string.IsNullOrEmpty(model.ConfirmPassword))
            {
                ShowErrorMessage("Please fill all required fields.");
                return View(model);
            }


            if (model.Password != model.ConfirmPassword)
            {
                ShowErrorMessage("Confirm password do not match.");
                return View(model);
            }


            var result = _repo.Create(model.ToDto());

            if (result.Success)
            {
                var user = (UserDto)result.Data ?? new UserDto();
                AuditLogRepo.CreateLog("Register", user.Id, user.Username, "Users", JsonConvert.SerializeObject(user));

                ShowMessage("Registration successful. You may now login.");
                return RedirectToAction("Login", "MyAccount", new { area = "customer" });
            }

            ShowErrorMessage(result.Message);
            return View(model);
        }

        public ActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (model.ConfirmNewPassword != model.NewPassword)
            {
                ShowErrorMessage("The new password and confirmation password do not match.");
                return View(new ChangePasswordViewModel());
            }

            if (model.CurrentPassword == model.NewPassword)
            {
                ShowErrorMessage("New password should not be the same as your previous password.");
                return View(new ChangePasswordViewModel());
            }
            var result = new UserRepo().ChangePassword(Identity.Id, model.CurrentPassword, model.NewPassword);


            if (result.Success)
            {
                FormsAuthentication.SignOut();
                TempData["ChangePassword"] = result.Message;
                TempData.Keep("ChangePassword");
                return RedirectToAction("Login");
                //return RedirectToAction("Index", "MyOrders", new { area = "customer" });
            }

            ShowMessage(result.Message, result.Success);
            return View(new ChangePasswordViewModel());
        }

        public ActionResult Logout()
        {
            _formsAuthenticationService.SignOut();
            return RedirectToAction("Index", "Home", new { area = "" });
        }

    }
}