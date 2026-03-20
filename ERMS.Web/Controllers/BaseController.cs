using ERMS.Web.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace ERMS.Web.Controllers
{
    public class BaseController : Controller
    {
        protected int CurrentUserId => SessionHelper.GetUserId(HttpContext.Session);
        protected string CurrentUser => HttpContext.Session.GetString(SessionHelper.UserName) ?? "";
        protected string CurrentFull => HttpContext.Session.GetString(SessionHelper.FullName) ?? "";
        protected bool IsAdmin => SessionHelper.IsAdmin(HttpContext.Session);
        protected bool IsLoggedIn => SessionHelper.IsLoggedIn(HttpContext.Session);

        protected string GetBURole(string buId) => SessionHelper.GetRole(HttpContext.Session, buId);

        protected IActionResult RedirectToLogin()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        protected bool CheckAuth()
        {
            if (!IsLoggedIn) return false;
            return true;
        }
    }
}
