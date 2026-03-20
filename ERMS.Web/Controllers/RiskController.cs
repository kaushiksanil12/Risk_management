using ERMS.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace ERMS.Web.Controllers
{
    public class RiskController : BaseController
    {
        private readonly IApiService _api;

        public RiskController(IApiService api)
        {
            _api = api;
        }

        public IActionResult Index()
        {
            if (!CheckAuth()) return RedirectToLogin();
            ViewData["Title"] = "Risk Register";
            return View();
        }

        public IActionResult Create()
        {
            if (!CheckAuth()) return RedirectToLogin();
            ViewData["Title"] = "Create New Risk";
            return View();
        }

        public IActionResult Edit(int id)
        {
            if (!CheckAuth()) return RedirectToLogin();
            ViewData["Title"] = "Edit Risk";
            ViewData["RiskId"] = id;
            return View();
        }

        public IActionResult Detail(int id)
        {
            if (!CheckAuth()) return RedirectToLogin();
            ViewData["Title"] = "Risk Details";
            ViewData["RiskId"] = id;
            return View();
        }

        public IActionResult Review(int id)
        {
            if (!CheckAuth()) return RedirectToLogin();
            ViewData["Title"] = "Review Risk";
            ViewData["RiskId"] = id;
            return View();
        }
    }
}
