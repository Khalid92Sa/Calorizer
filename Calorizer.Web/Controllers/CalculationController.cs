using Calorizer.Business.DTOs;
using Calorizer.Business.Interfaces;
using Calorizer.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Calorizer.Web.Controllers
{
    public class CalculationController : Controller
    {
        private readonly Localizer _localizer;
        private readonly ICalculationService _calculationService;

        public CalculationController(
            Localizer localizer,
            ICalculationService calculationService)
        {
            _localizer = localizer;
            _calculationService = calculationService;
        }

        // GET: Calculation/Index/5
        public async Task<IActionResult> Index(int id)
        {
            try
            {
                var calculation = await _calculationService.GetCalculationForClientAsync(id);
                return View(calculation);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = _localizer["ClientNotFound"];
                return RedirectToAction("Index", "Client");
            }
        }

        // POST: Calculation/Calculate
        [HttpPost]
        public IActionResult Calculate([FromBody] CalculationDto model)
        {
            try
            {
                // Calculate Total Energy Expenditure
                var tee = _calculationService.CalculateTotalEnergyExpenditure(
                    model.Weight,
                    model.Height,
                    model.Age,
                    model.GenderId,
                    model.PhysicalActivityFactor
                );

                model.TotalEnergyExpenditure = tee;

                // Calculate Macronutrient Distribution
                var result = _calculationService.CalculateMacronutrientDistribution(model);

                return Json(new
                {
                    success = true,
                    data = result
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = _localizer["ErrorOccurred"]
                });
            }
        }

        // POST: Calculation/CalculateFoodExchange
        [HttpPost]
        public IActionResult CalculateFoodExchange([FromBody] FoodExchangeListDto model)
        {
            try
            {
                // Calculate totals
                decimal totalCHO = 0;
                decimal totalProtein = 0;
                decimal totalFat = 0;
                decimal totalCalories = 0;

                // Calculate for each food item
                var items = new[]
                {
                    model.SkimmedMilk, model.LowFatMilk, model.FullFatMilk,
                    model.Fruit, model.Vegetable, model.Sugar, model.Starch,
                    model.LeanMeat, model.MediumFatMeat, model.HighFatMeat, model.Fat
                };

                foreach (var item in items)
                {
                    totalCHO += item.CHO * item.Serving;
                    totalProtein += item.Protein * item.Serving;
                    totalFat += item.Fat * item.Serving;
                    totalCalories += item.Calories * item.Serving;
                }

                model.TotalCHO = totalCHO;
                model.TotalProtein = totalProtein;
                model.TotalFat = totalFat;
                model.TotalCalories = totalCalories;

                return Json(new
                {
                    success = true,
                    data = model
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = _localizer["ErrorOccurred"]
                });
            }
        }
    }
}