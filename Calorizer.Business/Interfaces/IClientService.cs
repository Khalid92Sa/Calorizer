using Calorizer.Business.DTOs;
using Calorizer.Business.Models;

namespace Calorizer.Business.Interfaces
{
    public interface IClientService
    {
        Task<ClientDto?> GetClientByIdAsync(int id);
        Task<List<ClientDto>> GetAllClientsAsync();
        Task<Response<ClientDto>> CreateClientAsync(ClientDto clientDto, int userId);
        Task<Response<ClientDto>> UpdateClientAsync(ClientDto clientDto, int userId);
        Task DeleteClientAsync(int id);

        // Weight History
        Task<Response<List<WeightHistoryDto>>> AddWeightHistoryAsync(int clientId, WeightHistoryDto weightHistoryDto, int userId);
        Task<List<WeightHistoryDto>> GetWeightHistoriesAsync(int clientId);
        Task DeleteWeightHistoryAsync(int id);

        // Biochemical Tests
        Task<Response<List<BiochemicalMedicalTestDto>>> AddBiochemicalTestAsync(int clientId, BiochemicalMedicalTestDto testDto, int userId);
        Task<List<BiochemicalMedicalTestDto>> GetBiochemicalTestsAsync(int clientId);
        Task DeleteBiochemicalTestAsync(int id);

        // Drugs/Supplements
        Task<Response<List<DrugsSupplementDto>>> AddDrugsSupplementAsync(int clientId, DrugsSupplementDto drugDto, int userId);
        Task<List<DrugsSupplementDto>> GetDrugsSupplementsAsync(int clientId);
        Task DeleteDrugsSupplementAsync(int id);

        // Medical History
        Task<Response<List<MedicalHistoryDto>>> AddMedicalHistoryAsync(int clientId, MedicalHistoryDto medicalHistoryDto, int userId);
        Task<List<MedicalHistoryDto>> GetMedicalHistoriesAsync(int clientId);
        Task DeleteMedicalHistoryAsync(int id);
    }
}