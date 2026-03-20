using ERMS.Web.Helpers;
using ERMS.Web.Models;
using ERMS.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace ERMS.Web.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IApiService _api;

        public AccountController(IApiService api)
        {
            _api = api;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (IsLoggedIn) return RedirectToAction("Dashboard", "Home");
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _api.PostAsync<dynamic>("api/auth/login", new
            {
                username = model.Username,
                password = model.Password,
                loginType = model.LoginType
            });

            if (result == null)
            {
                model.ErrorMessage = "Unable to connect to the server.";
                return View(model);
            }

            bool success = (bool)(result.success ?? false);
            if (!success)
            {
                model.ErrorMessage = (string)(result.message ?? "Login failed.");
                return View(model);
            }

            var userData = result.data;
            SessionHelper.SetUser(HttpContext.Session, userData);

            // Load permissions
            int userId = (int)userData.userId;
            var permResult = await _api.GetAsync<dynamic>($"api/permission/byuser/{userId}");
            if (permResult != null && (bool)(permResult.success ?? false))
            {
                var permsJson = JsonConvert.SerializeObject(permResult.data);
                SessionHelper.SetPermissions(HttpContext.Session, permsJson);
            }

            return RedirectToAction("Dashboard", "Home");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            SessionHelper.Clear(HttpContext.Session);
            return RedirectToAction("Login");
        }
    }
}
