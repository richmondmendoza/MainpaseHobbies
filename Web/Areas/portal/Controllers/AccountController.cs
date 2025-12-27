using Dto.User;
using Newtonsoft.Json;
using Repository.Interfaces;
using Repository.Repo;
using Repository.Repo.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Web.App_Filters;
using Web.App_Filters.Services;
using Web.App_Interfaces;
using Web.Controllers;
using Web.Models;

namespace Web.Areas.portal.Controllers
{
    [PortalAuthorize]
    public class AccountController : BaseAdminController
    {
        UserRepo _repo = new UserRepo();

        [AllowAnonymous]
        public ActionResult Login(string returnUrl = "")
        {

            if (Identity != null)
            {
                return RedirectToAction("Index", "Dashboard", new { area = "portal" });
            }

            return View(new LoginViewModel()
            {
                ReturnUrl = returnUrl
            });
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

                if (!string.IsNullOrEmpty(model.ReturnUrl))
                {
                    return Redirect(model.ReturnUrl);
                }

                return RedirectToAction("Index", "Dashboard", new { area = "portal" });
            }

            ShowErrorMessage(result.Message);

            return View(model);
        }

        public ActionResult Logout()
        {
            _formsAuthenticationService.SignOut();
            return RedirectToAction("Index", "Home", new { area = "" });
        }


        public ActionResult List()
        {
            var users = new UserRepo().GetList();
            return View(users);
        }

        public ActionResult Add()
        {
            return View(new UserViewModel());
        }

        [HttpPost]
        public ActionResult Add(UserViewModel model)
        {
            var result = _repo.Create(model.ToDto());

            if (result.Success)
            {
                var user = (UserDto)result.Data;
                AuditLogRepo.CreateLog("Add User", Identity.Id, Identity.Username, "Users", JsonConvert.SerializeObject(user));

                ShowMessage(result.Message);
                return RedirectToAction("Details", new { id = user.Id });
            }

            ShowErrorMessage(result.Message);
            return View(new UserViewModel());
        }

        public ActionResult Details(int id)
        {
            var user = new UserRepo().Get(id);
            return View(new UserViewModel(user));
        }

        [HttpPost]
        public ActionResult Details(UserViewModel model)
        {
            var result = _repo.Update(model.ToDto());

            if (result.Success)
            {
                var user = (UserDto)result.Data;
                AuditLogRepo.CreateLog("Update User", Identity.Id, Identity.Username, "Users", JsonConvert.SerializeObject(user));

                ShowMessage(result.Message);
                return RedirectToAction("Details", new { id = user.Id });
            }

            ShowErrorMessage(result.Message);
            return View(new UserViewModel());
        }

        public ActionResult ChangePassword(int id)
        {
            var model = new ChangePasswordViewModel()
            {
                UserId = id
            };
            return PartialView("_ChangePassword", model);
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            var result = _repo.ChangePassword(model.UserId, model.NewPassword);

            if (result.Success)
            {
                var user = (UserDto)result.Data;
                AuditLogRepo.CreateLog("Password Reset", Identity.Id, Identity.Username, "Users", JsonConvert.SerializeObject(user));

                ShowMessage(result.Message);
                return RedirectToAction("Details", new { id = user.Id });
            }

            ShowErrorMessage(result.Message);
            return View(new UserViewModel());
        }



    }
}