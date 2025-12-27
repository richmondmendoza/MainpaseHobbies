using System.Web.Mvc;

namespace Web.Areas.portal
{
    public class portalAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "portal";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "portal_default",
                "portal/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}