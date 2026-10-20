using Calorizer.Business.Interfaces;
using Calorizer.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Calorizer.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDashboardService _dashboardService;

        public HomeController(ILogger<HomeController> logger, IDashboardService dashboardService)
        {
            _logger = logger;
            _dashboardService = dashboardService;
        }

        // GET: Home/Index (Dashboard)
        public async Task<IActionResult> Index()
        {
            var dashboard = await _dashboardService.GetDashboardAsync();
            return View(dashboard);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpGet]
        public IActionResult SetLanguage(string language, string returnUrl = "/")
        {
            if (language == "ar" || language == "en")
            {
                // Session
                HttpContext.Session.SetString("Language", language);

                // Cookie
                Response.Cookies.Append("Language", language, new CookieOptions
                {
                    Expires = DateTimeOffset.UtcNow.AddYears(1)
                });
            }

            return Redirect(returnUrl);
        }
    }
}
