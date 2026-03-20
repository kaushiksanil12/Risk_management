using ERMS.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace ERMS.Web.Controllers
{
    public class AdminController : BaseController
    {
        private readonly IApiService _api;

        public AdminController(IApiService api)
        {
            _api = api;
        }

        public IActionResult Users()
        {
            if (!CheckAuth()) return RedirectToLogin();
            if (!IsAdmin) return RedirectToAction("Dashboard", "Home");
            ViewData["Title"] = "User Management";
            return View();
        }

        public IActionResult BU()
        {
            if (!CheckAuth()) return RedirectToLogin();
            if (!IsAdmin) return RedirectToAction("Dashboard", "Home");
            ViewData["Title"] = "Business Units";
            return View();
        }

        public IActionResult RiskCategory()
        {
            if (!CheckAuth()) return RedirectToLogin();
            if (!IsAdmin) return RedirectToAction("Dashboard", "Home");
            ViewData["Title"] = "Risk Categories";
            return View();
        }

        public IActionResult RiskSubCategory()
        {
            if (!CheckAuth()) return RedirectToLogin();
            if (!IsAdmin) return RedirectToAction("Dashboard", "Home");
            ViewData["Title"] = "Risk Sub-Categories";
            return View();
        }

        public IActionResult Functions()
        {
            if (!CheckAuth()) return RedirectToLogin();
            if (!IsAdmin) return RedirectToAction("Dashboard", "Home");
            ViewData["Title"] = "Functions";
            return View();
        }

        public IActionResult Permissions(int userId)
        {
            if (!CheckAuth()) return RedirectToLogin();
            if (!IsAdmin) return RedirectToAction("Dashboard", "Home");
            ViewData["Title"] = "User Permissions";
            ViewData["TargetUserId"] = userId;
            return View();
        }
    }
}
