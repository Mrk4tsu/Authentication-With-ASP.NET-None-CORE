using Authentication.Models;
using Authentication.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace Authentication.Controllers
{
    public class BaseController : Controller
    {
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Request.Cookies[FormsAuthentication.FormsCookieName] == null)
            {
                filterContext.Result = RedirectToAction("Login", "Auths");
            }
            else
            {
                var authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                var authTicket = FormsAuthentication.Decrypt(authCookie.Value);

                if (authTicket == null || authTicket.Expired)
                {
                    filterContext.Result = RedirectToAction("Login", "Auths");
                }
            }
            base.OnActionExecuting(filterContext);
        }
    }
}