using ERMS.Web.Helpers;
using ERMS.Web.Models;
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
            ViewBag.CurrentUserId = CurrentUserId;
            ViewBag.IsAdmin = IsAdmin;
            return View();
        }

        // GET /Risk/Create — Owner or Admin only
        public IActionResult Create()
        {
            if (!CheckAuth()) return RedirectToLogin();

            // Role check happens after BU is selected — allow page load
            // but enforce on form submit via API
            ViewData["Title"] = "Create New Risk";
            return View();
        }

        // GET /Risk/Edit/{id} — Owner (own risk, Draft/Revision) or Admin only
        public async Task<IActionResult> Edit(int id)
        {
            if (!CheckAuth()) return RedirectToLogin();

            var risk = await _api.GetAsync<ApiResponse<RiskResponse>>($"/api/risk/{id}");
            if (risk?.Data == null) return NotFound();

            // Block if not Draft or RevisionRequired
            if (risk.Data.Status != "Draft" && risk.Data.Status != "RevisionRequired")
            {
                TempData["Error"] = "This risk cannot be edited in its current status.";
                return RedirectToAction("Detail", new { id });
            }

            // Block if not Owner of this BU and not Admin
            if (!IsAdmin)
            {
                var role = SessionHelper.GetRole(HttpContext.Session, risk.Data.BUId);
                if (role != "O" || risk.Data.CreatedBy != CurrentUserId)
                {
                    TempData["Error"] = "Access denied. You can only edit risks you created.";
                    return RedirectToAction("Detail", new { id });
                }
            }

            ViewData["Title"] = "Edit Risk";
            ViewData["RiskId"] = risk.Data.RiskId;
            return View(risk.Data);
        }

        public async Task<IActionResult> Detail(int id)
        {
            if (!CheckAuth()) return RedirectToLogin();

            var risk = await _api.GetAsync<ApiResponse<RiskResponse>>($"/api/risk/{id}");
            if (risk?.Data == null) return NotFound();

            ViewData["Title"] = "Risk Details";
            ViewBag.CurrentUserId = CurrentUserId;
            ViewBag.IsAdmin = IsAdmin;
            return View(risk.Data);
        }

        // GET /Risk/Review/{id} — Champion or Admin only
        public async Task<IActionResult> Review(int id)
        {
            if (!CheckAuth()) return RedirectToLogin();

            var risk = await _api.GetAsync<ApiResponse<RiskResponse>>($"/api/risk/{id}");
            if (risk?.Data == null) return NotFound();

            if (!IsAdmin)
            {
                var role = SessionHelper.GetRole(HttpContext.Session, risk.Data.BUId);
                if (role != "C")
                {
                    TempData["Error"] = "Access denied. Only Risk Champions can review risks.";
                    return RedirectToAction("Detail", new { id });
                }
            }

            ViewData["Title"] = "Review Risk";
            ViewBag.IsAdmin = IsAdmin;
            return View(risk.Data);
        }
    }
}
