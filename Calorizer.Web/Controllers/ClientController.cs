using Calorizer.Business.DTOs;
using Calorizer.Business.Enums;
using Calorizer.Business.Interfaces;
using Calorizer.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace Calorizer.Web.Controllers
{
    public class ClientController : Controller
    {
        private readonly Localizer _localizer;
        private readonly IClientService _clientService;
        private readonly ILookupService _lookupService;

        public ClientController(
            Localizer localizer,
            IClientService clientService,
            ILookupService lookupService)
        {
            _localizer = localizer;
            _clientService = clientService;
            _lookupService = lookupService;
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
            try
            {
                int userId = 1; // TODO: Get actual user ID from authentication

                var result = await _clientService.CreateClientAsync(model, userId);

                if (result.Succeeded)
                {
                    string msg = _localizer["ClientCreatedSuccessfully"];
                    return Json(new
                    {
                        isSuccess = true,
                        resultCode = result.StatusCode,
                        msg = msg,
                        redirectUrl = Url.Action(nameof(Edit), new { id = result.Data?.Id })
                    });
                }

                return Json(new
                {
                    isSuccess = false,
                    resultCode = result.StatusCode,
                    brokenRules = result.BrokenRules,
                    msg = _localizer[result.Message]
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSuccess = false,
                    msg = _localizer["ErrorOccurred"]
                });
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
            try
            {
                int userId = 1; // TODO: Get actual user ID from authentication

                var result = await _clientService.UpdateClientAsync(model, userId);

                if (result.Succeeded)
                {
                    string msg = _localizer["ClientUpdatedSuccessfully"];
                    return Json(new
                    {
                        isSuccess = true,
                        resultCode = result.StatusCode,
                        msg = msg
                    });
                }

                return Json(new
                {
                    isSuccess = false,
                    resultCode = result.StatusCode,
                    brokenRules = result.BrokenRules,
                    msg = _localizer[result.Message]
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    isSuccess = false,
                    msg = _localizer["ErrorOccurred"]
                });
            }
        }

        #region Weight History AJAX Actions

        [HttpPost]
        public async Task<IActionResult> AddWeightHistory([FromBody] WeightHistoryDto model)
        {
            try
            {
                int userId = 1;
                var result = await _clientService.AddWeightHistoryAsync(model.ClientId, model, userId);

                if (result.Succeeded)
                {
                    return Json(new
                    {
                        success = true,
                        data = result.Data,
                        msg = _localizer["WeightHistoryAdded"]
                    });
                }

                return Json(new
                {
                    success = false,
                    brokenRules = result.BrokenRules,
                    message = _localizer[result.Message]
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

        [HttpPost]
        public async Task<IActionResult> DeleteWeightHistory(int id, int clientId)
        {
            try
            {
                await _clientService.DeleteWeightHistoryAsync(id);
                var histories = await _clientService.GetWeightHistoriesAsync(clientId);
                return Json(new
                {
                    success = true,
                    data = histories,
                    msg = _localizer["WeightHistoryDeleted"]
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

        #endregion

        #region Biochemical Test AJAX Actions

        [HttpPost]
        public async Task<IActionResult> AddBiochemicalTest([FromBody] BiochemicalMedicalTestDto model)
        {
            try
            {
                int userId = 1;
                var result = await _clientService.AddBiochemicalTestAsync(model.ClientId, model, userId);

                if (result.Succeeded)
                {
                    return Json(new
                    {
                        success = true,
                        data = result.Data,
                        msg = _localizer["BiochemicalTestAdded"]
                    });
                }

                return Json(new
                {
                    success = false,
                    brokenRules = result.BrokenRules,
                    message = _localizer[result.Message]
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

        [HttpPost]
        public async Task<IActionResult> DeleteBiochemicalTest(int id, int clientId)
        {
            try
            {
                await _clientService.DeleteBiochemicalTestAsync(id);
                var tests = await _clientService.GetBiochemicalTestsAsync(clientId);
                return Json(new
                {
                    success = true,
                    data = tests,
                    msg = _localizer["BiochemicalTestDeleted"]
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

        #endregion

        #region Drugs/Supplements AJAX Actions

        [HttpPost]
        public async Task<IActionResult> AddDrugsSupplement([FromBody] DrugsSupplementDto model)
        {
            try
            {
                int userId = 1;
                var result = await _clientService.AddDrugsSupplementAsync(model.ClientId, model, userId);

                if (result.Succeeded)
                {
                    return Json(new
                    {
                        success = true,
                        data = result.Data,
                        msg = _localizer["DrugsSupplementAdded"]
                    });
                }

                return Json(new
                {
                    success = false,
                    brokenRules = result.BrokenRules,
                    message = _localizer[result.Message]
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

        [HttpPost]
        public async Task<IActionResult> DeleteDrugsSupplement(int id, int clientId)
        {
            try
            {
                await _clientService.DeleteDrugsSupplementAsync(id);
                var drugs = await _clientService.GetDrugsSupplementsAsync(clientId);
                return Json(new
                {
                    success = true,
                    data = drugs,
                    msg = _localizer["DrugsSupplementDeleted"]
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

        #endregion

        #region Medical History AJAX Actions

        [HttpPost]
        public async Task<IActionResult> AddMedicalHistory([FromBody] MedicalHistoryDto model)
        {
            try
            {
                int userId = 1;
                var result = await _clientService.AddMedicalHistoryAsync(model.ClientId, model, userId);

                if (result.Succeeded)
                {
                    return Json(new
                    {
                        success = true,
                        data = result.Data,
                        msg = _localizer["MedicalHistoryAdded"]
                    });
                }

                return Json(new
                {
                    success = false,
                    brokenRules = result.BrokenRules,
                    message = _localizer[result.Message]
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

        [HttpPost]
        public async Task<IActionResult> DeleteMedicalHistory(int id, int clientId)
        {
            try
            {
                await _clientService.DeleteMedicalHistoryAsync(id);
                var histories = await _clientService.GetMedicalHistoriesAsync(clientId);
                return Json(new
                {
                    success = true,
                    data = histories,
                    msg = _localizer["MedicalHistoryDeleted"]
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

        #endregion
    }
}