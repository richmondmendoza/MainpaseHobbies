using Dto.Enums;
using Dto.User;
using Infrastructure;
using Newtonsoft.Json;
using Repository.Interfaces;
using Repository.Repo.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.SessionState;

namespace Web.App_Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public class PortalAuthorizeAttribute : AuthorizeAttribute
    {
        ISessionManager _sessionManager = new SessionManagerRepo();

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            if (filterContext.Result == null ||
                (filterContext.Result.GetType() != typeof(HttpUnauthorizedResult)
                                                 || !filterContext.HttpContext.Request.IsAjaxRequest()))
                return;

            var urlHelper = new UrlHelper(filterContext.RequestContext);

            filterContext.Result = new JavaScriptResult() { Script = "window.location = '" + urlHelper.Action("login", "account", new { area = "portal" }) + "'" };
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
                throw new ArgumentNullException("httpContext");

            if (!httpContext.User.Identity.IsAuthenticated)
                return RedirectUnauthorized(httpContext);

            var identity = (FormsIdentity)httpContext.User.Identity;


            if (!identity.IsAuthenticated)
                return RedirectUnauthorized(httpContext);


            var ticket = identity.Ticket;

            var decryptedData = Fletcher.Decrypt(ticket.UserData);

            var userInfo = JsonConvert.DeserializeObject<AuthenticatedUserDto>(decryptedData);

            var session = _sessionManager.ValidateSession(userInfo.Id, userInfo.AccessToken);

            if (!session.Success)
            {
                HttpContext.Current.Session["TimeOutMessage"] = session.Message;
                return false;
            }

            var roles = SplitString(Roles, '|');

            if (roles == null || roles.Length == 0)
                return true;


            if (userInfo.HasAdminAccess)
                return true;

            HttpContext.Current.Session["TimeOutMessage"] = "Current user has no access to this site.";
            return false;
        }

        public bool RedirectUnauthorized(HttpContextBase httpContext)
        {
            HttpCookie myCookie = httpContext.Request.Cookies["FDLS_User"];
            if (myCookie != null)
            {
                if (!string.IsNullOrEmpty(myCookie.Values["XID"]))
                {
                    var empID = Convert.ToInt32(myCookie.Values["XID"]);

                    var lastSession = _sessionManager.Get(empID);

                    if (lastSession != null)
                    {
                        httpContext.Request.Cookies.Remove("FDLS_User");
                        HttpContext.Current.Session["TimeOutMessage"] = "Session Expired [USR:" + lastSession.FullName + "]  [LAD: " + lastSession.LastDateOfActivity + "]";
                    }
                }
            }
            return false;
        }

        internal static string[] SplitString(string original, char separator = ',')
        {
            if (string.IsNullOrEmpty(original))
            {
                return new string[0];
            }

            var split = from piece in original.Split(separator)
                        let trimmed = piece.Trim()
                        where !string.IsNullOrEmpty(trimmed)
                        select trimmed;
            return split.ToArray();
        }
    }
}