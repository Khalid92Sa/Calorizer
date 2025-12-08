using Calorizer.Business.Interfaces;
using Calorizer.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Calorizer.Web.Controllers
{
    public class ClientController : Controller
    {
        readonly Localizer _localizer;
        public ClientController(Localizer localizer)
        {
            _localizer = localizer;
        }
        public IActionResult Index()
        {
            ViewBag.PageTitle = _localizer["FoodName"];

            return View();
        }
    }
}
