using Dto.User;
using Infrastructure;
using Newtonsoft.Json;
using Repository.Repo;
using Repository.Repo.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Security;
using Web.App_Interfaces;
using Web.Controllers;

namespace Web.App_Filters.Services
{
    public class FormsAuthenticationServices : IFormsAuthenticationService
    {
        public void SetAuthCookie(AuthenticatedUserDto user, bool createPersistentCookie)
        {
            var userData = JsonConvert.SerializeObject(user);

            var encryptedData = Fletcher.Encrypt(userData);

            var ticket = new FormsAuthenticationTicket(
                version: 1,
                name: user.Username,
                issueDate: DateTime.Now,
                expiration: createPersistentCookie ? DateTime.Now.AddYears(1) : DateTime.Now.AddMinutes(36),
                isPersistent: createPersistentCookie,
                userData: encryptedData
                );

            //var roles = user.Roles.ToArray();

            var userPrincipal = new GenericPrincipal(new GenericIdentity(ticket.Name), new string[] { });

            HttpContext.Current.User = userPrincipal;

            var encryptedTicket = FormsAuthentication.Encrypt(ticket);

            var context = HttpContext.Current;

            var formsCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);

            context.Response.Cookies.Add(formsCookie);

            var userSessionKey = context.Request.Cookies["Microsoft.Application.Key"].Value;

            if (userSessionKey != null)
            {
                context.Request.Cookies.Remove(userSessionKey);
                BaseController.UserSessionKey = "";
            }
        }

        public void SetUserSessionKey()
        {
            var context = HttpContext.Current;
            var exists = context.Request.Cookies["Microsoft.Application.Key"] != null;
            if (exists)
            {
                BaseController.UserSessionKey = context.Request.Cookies["Microsoft.Application.Key"].Value;
            }
            else
            {
                BaseController.UserSessionKey = Guid.NewGuid().ToString();
                HttpCookie cookie = new HttpCookie("Microsoft.Application.Key", BaseController.UserSessionKey);
                cookie.Expires = DateTime.Now.AddDays(7);
                context.Response.Cookies.Add(cookie);
            }

        }

        public AuthenticatedUserDto Identity()
        {
            if (!HttpContext.Current.User.Identity.IsAuthenticated)
                return null;

            FormsIdentity identity = (FormsIdentity)HttpContext.Current.User.Identity;

            if (!identity.IsAuthenticated)
                return null;

            var ticket = identity.Ticket;

            var decryptedData = Fletcher.Decrypt(ticket.UserData);

            var userInfo = JsonConvert.DeserializeObject<AuthenticatedUserDto>(decryptedData);

            return userInfo;
        }


        public void SignOut()
        {
            var user = Identity();

            AuditLogRepo.CreateLog("Logout", user.Id, user.Username, "Users", JsonConvert.SerializeObject(user));
            new SessionManagerRepo().DeleteUserSession(user.Id);

            HttpContext.Current.Request.Cookies.Clear();
            HttpContext.Current.Session.Clear();
            HttpContext.Current.Session.Abandon();
            FormsAuthentication.SignOut();
        }
    }
}