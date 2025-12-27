using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Web.Utils
{
    public static class HtmlHelperExtensions
    {
        public static IHtmlString RouteIf(this HtmlHelper helper, string areaName, string controller, string action, string attribute)
        {
            var actions = action.Split('|').Where(a => !string.IsNullOrEmpty(a));
            var controllers = controller.Split('|').Where(a => !string.IsNullOrEmpty(a));
            var areas = areaName.Split('|').Where(a => !string.IsNullOrEmpty(a));

            var currentAction =
                (helper.ViewContext.RequestContext.RouteData.Values["action"] ?? string.Empty).ToString();

            var currentController =
                (helper.ViewContext.RequestContext.RouteData.Values["controller"] ?? string.Empty).ToString();

            var currentArea =
                (helper.ViewContext.RequestContext.RouteData.DataTokens["area"] ?? string.Empty).ToString();

            var hasArea = areas.Where(a => a.ToLower().Equals(currentArea.ToLower())).Any() ? true :
                string.IsNullOrEmpty(currentArea) & !areas.Any();

            var hasController = controllers.Where(a => a.ToLower().Equals(currentController.ToLower())).Any();

            var hasAction = actions.Where(a => a.ToLower().Equals(currentAction.ToLower())).Any();

            return hasArea & hasController & hasAction ? new HtmlString(attribute) : new HtmlString(string.Empty);
        }
    }
}