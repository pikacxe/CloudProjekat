using System.Web;
using System.Web.Mvc;

namespace PortfolioService
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new AuthorizeUserAttribute());
            filters.Add(new HandleErrorAttribute());
        }

    }
    public class AuthorizeUserAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            /// If action is LogIn skip the filter
            var actionDescriptor = filterContext.ActionDescriptor;
            var actionName = actionDescriptor.ActionName;
            var controllerName = actionDescriptor.ControllerDescriptor.ControllerName;

            if (controllerName == "User" && actionName == "LogIn")
                return;
            

            /// Check if user is logged in
            if (filterContext.HttpContext.Session["LoggedInUserEmail"] == null)
            {
                filterContext.Result = new RedirectResult("~/User/LogIn");
                base.OnActionExecuting(filterContext);
                return;
            }
        }
    }
}
