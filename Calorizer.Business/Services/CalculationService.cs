using Calorizer.Business.DTOs;
using Calorizer.Business.Enums;
using Calorizer.Business.Interfaces;
using Calorizer.Business.Models;
using Calorizer.DAL.Models;
using Calorizer.DAL.Repositories;

namespace Calorizer.Business.Services
{
    public class CalculationService : ICalculationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILookupService _lookupService;

        public CalculationService(IUnitOfWork unitOfWork, ILookupService lookupService)
        {
            _unitOfWork = unitOfWork;
            _lookupService = lookupService;
        }

        public async Task<CalculationDto> GetCalculationForClientAsync(int clientId)
        {
            var client = await _unitOfWork.Repository<Client>().GetByIdAsync(clientId);
            if (client == null)
                throw new Exception("Client not found");

            var calculation = new CalculationDto
            {
                ClientId = client.Id,
                ClientName = client.FullNameEn,
                Height = client.Height ?? 0,
                Weight = client.Weight ?? 0,
                Age = CalculateAge(client.DateOfBirth),
                GenderId = client.GenderId,
                PhysicalActivityFactor = 1.2m, // Default value
                PhysicalActivityFactors = await _lookupService.GetLookupItems(LookupTypes.PhysicalActivityFactor)
            };

            return calculation;
        }

        public decimal CalculateTotalEnergyExpenditure(decimal weight, decimal height, int age, int genderId, decimal physicalActivityFactor)
        {
            // Get gender lookup to determine if male or female
            // Assuming GenderId 1 = Male, 2 = Female (adjust based on your data)
            decimal bmr;

            // Male formula: ((10 * weight) + (6.25 * height) - (5 * age) + 5) * physical activity
            // Female formula: ((10 * weight) + (6.25 * height) - (5 * age) - 161) * physical activity

            if (genderId == 1) // Male
            {
                bmr = (10m * weight) + (6.25m * height) - (5m * age) + 5m;
            }
            else // Female
            {
                bmr = (10m * weight) + (6.25m * height) - (5m * age) - 161m;
            }

            return bmr * physicalActivityFactor;
        }

        public CalculationDto CalculateMacronutrientDistribution(CalculationDto calculation)
        {
            calculation.TotalCalories = calculation.TotalEnergyExpenditure;

            // Calculate Carbohydrates (4 calories per gram)
            calculation.CarbsGramsMin = (calculation.TotalCalories * (calculation.CarbsPercentageMin / 100m)) / 4m;
            calculation.CarbsGramsMax = (calculation.TotalCalories * (calculation.CarbsPercentageMax / 100m)) / 4m;

            // Calculate Protein (4 calories per gram)
            calculation.ProteinGramsMin = (calculation.TotalCalories * (calculation.ProteinPercentageMin / 100m)) / 4m;
            calculation.ProteinGramsMax = (calculation.TotalCalories * (calculation.ProteinPercentageMax / 100m)) / 4m;

            // Calculate Fat (9 calories per gram)
            calculation.FatGramsMin = (calculation.TotalCalories * (calculation.FatPercentageMin / 100m)) / 9m;
            calculation.FatGramsMax = (calculation.TotalCalories * (calculation.FatPercentageMax / 100m)) / 9m;

            return calculation;
        }

        public async Task<Response<CalculationDto>> SaveCalculationAsync(CalculationDto calculationDto, int userId)
        {
            try
            {
                // Here you can save calculation results to database if needed
                // For now, we'll just return success with the calculated data

                return new Response<CalculationDto>
                {
                    Succeeded = true,
                    Data = calculationDto,
                    StatusCode = HttpStatusCode.OK
                };
            }
            catch (Exception ex)
            {
                return new Response<CalculationDto>
                {
                    Succeeded = false,
                    Message = "ErrorSavingCalculation",
                    StatusCode = HttpStatusCode.BadRequest
                };
            }
        }

        private int CalculateAge(DateTime dateOfBirth)
        {
            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}