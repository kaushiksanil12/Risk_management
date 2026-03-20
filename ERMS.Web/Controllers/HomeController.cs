using ERMS.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace ERMS.Web.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IApiService _api;

        public HomeController(IApiService api)
        {
            _api = api;
        }

        public IActionResult Dashboard()
        {
            if (!CheckAuth()) return RedirectToLogin();
            ViewData["Title"] = "Dashboard";
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
