using Calorizer.Business.DTOs;
using Calorizer.Business.Models;

namespace Calorizer.Business.Interfaces
{
    public interface ICalculationService
    {
        Task<CalculationDto> GetCalculationForClientAsync(int clientId);
        decimal CalculateTotalEnergyExpenditure(decimal weight, decimal height, int age, int genderId, decimal physicalActivityFactor);
        CalculationDto CalculateMacronutrientDistribution(CalculationDto calculation);
        Task<Response<CalculationDto>> SaveCalculationAsync(CalculationDto calculationDto, int userId);

        // Calculation Notes (history)
        Task<Response<List<CalculationNoteDto>>> AddNoteAsync(CalculationNoteDto noteDto, int userId);
        Task<List<CalculationNoteDto>> GetNotesAsync(int clientId);
        Task DeleteNoteAsync(int id);
    }
}