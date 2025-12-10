using Calorizer.Business.DTOs;
using Calorizer.Business.DTOs.Validations;
using Calorizer.Business.Enums;
using Calorizer.Business.Interfaces;
using Calorizer.Business.Services;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Calorizer.Web.Controllers
{
    public class ClientController : Controller
    {
        private readonly Localizer _localizer;
        private readonly IClientService _clientService;
        private readonly ILookupService _lookupService;
        private readonly IValidator<ClientDto> _clientValidator;
        private readonly IValidator<WeightHistoryDto> _weightHistoryValidator;
        private readonly IValidator<BiochemicalMedicalTestDto> _biochemicalTestValidator;
        private readonly IValidator<DrugsSupplementDto> _drugsSupplementValidator;
        private readonly IValidator<MedicalHistoryDto> _medicalHistoryValidator;

        public ClientController(
            Localizer localizer,
            IClientService clientService,
            ILookupService lookupService,
            IValidator<ClientDto> clientValidator,
            IValidator<WeightHistoryDto> weightHistoryValidator,
            IValidator<BiochemicalMedicalTestDto> biochemicalTestValidator,
            IValidator<DrugsSupplementDto> drugsSupplementValidator,
            IValidator<MedicalHistoryDto> medicalHistoryValidator)
        {
            _localizer = localizer;
            _clientService = clientService;
            _lookupService = lookupService;
            _clientValidator = clientValidator;
            _weightHistoryValidator = weightHistoryValidator;
            _biochemicalTestValidator = biochemicalTestValidator;
            _drugsSupplementValidator = drugsSupplementValidator;
            _medicalHistoryValidator = medicalHistoryValidator;
        }

        // GET: Client/Index
        public async Task<IActionResult> Index()
        {
            var clients = await _clientService.GetAllClientsAsync();
            return View(clients);
        }

        // GET: Client/Create
        public async Task<IActionResult> Create()
        {
            var model = new ClientDto
            {
                Genders = await _lookupService.GetLookupItems(LookupTypes.Gender),
                DateOfBirth = DateTime.Today.AddYears(-25)
            };
            return View(model);
        }

        // POST: Client/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClientDto model)
        {
            var validationResult = await _clientValidator.ValidateAsync(model);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                model.Genders = await _lookupService.GetLookupItems(LookupTypes.Gender);
                return View(model);
            }

            try
            {
                // TODO: Get actual user ID from authentication
                int userId = 1;

                var createdClient = await _clientService.CreateClientAsync(model, userId);
                TempData["SuccessMessage"] = "Client created successfully";
                return RedirectToAction(nameof(Edit), new { id = createdClient.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error creating client: {ex.Message}");
                model.Genders = await _lookupService.GetLookupItems(LookupTypes.Gender);
                return View(model);
            }
        }

        // GET: Client/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var client = await _clientService.GetClientByIdAsync(id);
            if (client == null)
            {
                return NotFound();
            }

            client.Genders = await _lookupService.GetLookupItems(LookupTypes.Gender);
            client.WeightHistories = await _clientService.GetWeightHistoriesAsync(id);
            client.BiochemicalTests = await _clientService.GetBiochemicalTestsAsync(id);
            client.DrugsSupplements = await _clientService.GetDrugsSupplementsAsync(id);
            client.MedicalHistories = await _clientService.GetMedicalHistoriesAsync(id);

            return View(client);
        }

        // POST: Client/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ClientDto model)
        {
            var validationResult = await _clientValidator.ValidateAsync(model);

            if (!validationResult.IsValid)
            {
                foreach (var error in validationResult.Errors)
                {
                    ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
                }
                model.Genders = await _lookupService.GetLookupItems(LookupTypes.Gender);
                model.WeightHistories = await _clientService.GetWeightHistoriesAsync(model.Id);
                model.BiochemicalTests = await _clientService.GetBiochemicalTestsAsync(model.Id);
                model.DrugsSupplements = await _clientService.GetDrugsSupplementsAsync(model.Id);
                model.MedicalHistories = await _clientService.GetMedicalHistoriesAsync(model.Id);
                return View(model);
            }

            try
            {
                // TODO: Get actual user ID from authentication
                int userId = 1;

                await _clientService.UpdateClientAsync(model, userId);
                TempData["SuccessMessage"] = "Client updated successfully";
                return RedirectToAction(nameof(Edit), new { id = model.Id });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error updating client: {ex.Message}");
                model.Genders = await _lookupService.GetLookupItems(LookupTypes.Gender);
                model.WeightHistories = await _clientService.GetWeightHistoriesAsync(model.Id);
                model.BiochemicalTests = await _clientService.GetBiochemicalTestsAsync(model.Id);
                model.DrugsSupplements = await _clientService.GetDrugsSupplementsAsync(model.Id);
                model.MedicalHistories = await _clientService.GetMedicalHistoriesAsync(model.Id);
                return View(model);
            }
        }

        #region Weight History AJAX Actions

        [HttpPost]
        public async Task<IActionResult> AddWeightHistory([FromBody] WeightHistoryDto model)
        {
            var validationResult = await _weightHistoryValidator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                return Json(new { success = false, message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)) });
            }

            try
            {
                int userId = 1; // TODO: Get from authentication
                await _clientService.AddWeightHistoryAsync(model.ClientId, model, userId);
                var histories = await _clientService.GetWeightHistoriesAsync(model.ClientId);
                return Json(new { success = true, data = histories });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteWeightHistory(int id, int clientId)
        {
            try
            {
                await _clientService.DeleteWeightHistoryAsync(id);
                var histories = await _clientService.GetWeightHistoriesAsync(clientId);
                return Json(new { success = true, data = histories });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Biochemical Test AJAX Actions

        [HttpPost]
        public async Task<IActionResult> AddBiochemicalTest([FromBody] BiochemicalMedicalTestDto model)
        {
            var validationResult = await _biochemicalTestValidator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                return Json(new { success = false, message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)) });
            }

            try
            {
                int userId = 1;
                await _clientService.AddBiochemicalTestAsync(model.ClientId, model, userId);
                var tests = await _clientService.GetBiochemicalTestsAsync(model.ClientId);
                return Json(new { success = true, data = tests });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteBiochemicalTest(int id, int clientId)
        {
            try
            {
                await _clientService.DeleteBiochemicalTestAsync(id);
                var tests = await _clientService.GetBiochemicalTestsAsync(clientId);
                return Json(new { success = true, data = tests });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Drugs/Supplements AJAX Actions

        [HttpPost]
        public async Task<IActionResult> AddDrugsSupplement([FromBody] DrugsSupplementDto model)
        {
            var validationResult = await _drugsSupplementValidator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                return Json(new { success = false, message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)) });
            }

            try
            {
                int userId = 1;
                await _clientService.AddDrugsSupplementAsync(model.ClientId, model, userId);
                var drugs = await _clientService.GetDrugsSupplementsAsync(model.ClientId);
                return Json(new { success = true, data = drugs });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDrugsSupplement(int id, int clientId)
        {
            try
            {
                await _clientService.DeleteDrugsSupplementAsync(id);
                var drugs = await _clientService.GetDrugsSupplementsAsync(clientId);
                return Json(new { success = true, data = drugs });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion

        #region Medical History AJAX Actions

        [HttpPost]
        public async Task<IActionResult> AddMedicalHistory([FromBody] MedicalHistoryDto model)
        {
            var validationResult = await _medicalHistoryValidator.ValidateAsync(model);
            if (!validationResult.IsValid)
            {
                return Json(new { success = false, message = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)) });
            }

            try
            {
                int userId = 1;
                await _clientService.AddMedicalHistoryAsync(model.ClientId, model, userId);
                var histories = await _clientService.GetMedicalHistoriesAsync(model.ClientId);
                return Json(new { success = true, data = histories });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteMedicalHistory(int id, int clientId)
        {
            try
            {
                await _clientService.DeleteMedicalHistoryAsync(id);
                var histories = await _clientService.GetMedicalHistoriesAsync(clientId);
                return Json(new { success = true, data = histories });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        #endregion
    }
}