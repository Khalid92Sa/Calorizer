using Calorizer.Business.DTOs;
using Calorizer.Business.Interfaces;
using Calorizer.DAL.Models;
using Calorizer.DAL.Repositories;

namespace Calorizer.Business.Services
{
    public class ClientService : IClientService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ClientService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ClientDto?> GetClientByIdAsync(int id)
        {
            var client = await _unitOfWork.Repository<Client>().GetByIdAsync(id);
            if (client == null) return null;

            return new ClientDto
            {
                Id = client.Id,
                FullNameEn = client.FullNameEn,
                FullNameAr = client.FullNameAr,
                MobileNumber = client.MobileNumber,
                GenderId = client.GenderId,
                Address = client.Address,
                DateOfBirth = client.DateOfBirth,
                Weight = client.Weight,
                Height = client.Height
            };
        }

        public async Task<List<ClientDto>> GetAllClientsAsync()
        {
            var clients = await _unitOfWork.Repository<Client>().GetAllAsync();
            return clients.Select(c => new ClientDto
            {
                Id = c.Id,
                FullNameEn = c.FullNameEn,
                FullNameAr = c.FullNameAr,
                MobileNumber = c.MobileNumber,
                GenderId = c.GenderId,
                Address = c.Address,
                DateOfBirth = c.DateOfBirth,
                Weight = c.Weight,
                Height = c.Height
            }).ToList();
        }

        public async Task<ClientDto> CreateClientAsync(ClientDto clientDto, int userId)
        {
            var client = new Client
            {
                FullNameEn = clientDto.FullNameEn,
                FullNameAr = clientDto.FullNameAr,
                MobileNumber = clientDto.MobileNumber,
                GenderId = clientDto.GenderId,
                Address = clientDto.Address,
                DateOfBirth = clientDto.DateOfBirth,
                Weight = clientDto.Weight,
                Height = clientDto.Height
            };

            await _unitOfWork.Repository<Client>().AddAsync(client);

            // Add initial weight history if weight/height provided
            if (clientDto.Weight.HasValue || clientDto.Height.HasValue)
            {
                var weightHistory = new WeightHistory
                {
                    ClientId = client.Id,
                    Weight = clientDto.Weight,
                    Height = clientDto.Height,
                    CreatedBy = userId,
                    CreatedOn = DateTime.Now
                };
                await _unitOfWork.Repository<WeightHistory>().AddAsync(weightHistory);
            }

            clientDto.Id = client.Id;
            return clientDto;
        }

        public async Task<ClientDto> UpdateClientAsync(ClientDto clientDto, int userId)
        {
            var client = await _unitOfWork.Repository<Client>().GetByIdAsync(clientDto.Id);
            if (client == null)
                throw new KeyNotFoundException($"Client with ID {clientDto.Id} not found");

            // Check if weight or height changed
            bool weightChanged = client.Weight != clientDto.Weight;
            bool heightChanged = client.Height != clientDto.Height;

            client.FullNameEn = clientDto.FullNameEn;
            client.FullNameAr = clientDto.FullNameAr;
            client.MobileNumber = clientDto.MobileNumber;
            client.GenderId = clientDto.GenderId;
            client.Address = clientDto.Address;
            client.DateOfBirth = clientDto.DateOfBirth;
            client.Weight = clientDto.Weight;
            client.Height = clientDto.Height;

            await _unitOfWork.Repository<Client>().UpdateAsync(client);

            // Add weight history record if changed
            if (weightChanged || heightChanged)
            {
                var weightHistory = new WeightHistory
                {
                    ClientId = client.Id,
                    Weight = clientDto.Weight,
                    Height = clientDto.Height,
                    CreatedBy = userId,
                    CreatedOn = DateTime.Now
                };
                await _unitOfWork.Repository<WeightHistory>().AddAsync(weightHistory);
            }

            return clientDto;
        }

        public async Task DeleteClientAsync(int id)
        {
            await _unitOfWork.Repository<Client>().DeleteAsync(id);
        }

        #region Weight History

        public async Task AddWeightHistoryAsync(int clientId, WeightHistoryDto weightHistoryDto, int userId)
        {
            var weightHistory = new WeightHistory
            {
                ClientId = clientId,
                Weight = weightHistoryDto.Weight,
                Height = weightHistoryDto.Height,
                CreatedBy = userId,
                CreatedOn = DateTime.Now
            };

            await _unitOfWork.Repository<WeightHistory>().AddAsync(weightHistory);

            // Update client's current weight/height
            var client = await _unitOfWork.Repository<Client>().GetByIdAsync(clientId);
            if (client != null)
            {
                client.Weight = weightHistoryDto.Weight;
                client.Height = weightHistoryDto.Height;
                await _unitOfWork.Repository<Client>().UpdateAsync(client);
            }
        }

        public async Task<List<WeightHistoryDto>> GetWeightHistoriesAsync(int clientId)
        {
            var histories = await _unitOfWork.Repository<WeightHistory>()
                .FindAsync(w => w.ClientId == clientId);

            return histories.Select(w => new WeightHistoryDto
            {
                Id = w.Id,
                ClientId = w.ClientId,
                Weight = w.Weight,
                Height = w.Height,
                CreatedOn = w.CreatedOn,
                CreatedBy = w.CreatedBy
            }).OrderByDescending(w => w.CreatedOn).ToList();
        }

        public async Task DeleteWeightHistoryAsync(int id)
        {
            await _unitOfWork.Repository<WeightHistory>().DeleteAsync(id);
        }

        #endregion

        #region Biochemical Tests

        public async Task AddBiochemicalTestAsync(int clientId, BiochemicalMedicalTestDto testDto, int userId)
        {
            var test = new BiochemicalMedicalTest
            {
                ClientId = clientId,
                MedicalData = testDto.MedicalData,
                CreatedBy = userId,
                CreatedOn = DateTime.Now
            };

            await _unitOfWork.Repository<BiochemicalMedicalTest>().AddAsync(test);
        }

        public async Task<List<BiochemicalMedicalTestDto>> GetBiochemicalTestsAsync(int clientId)
        {
            var tests = await _unitOfWork.Repository<BiochemicalMedicalTest>()
                .FindAsync(b => b.ClientId == clientId);

            return tests.Select(t => new BiochemicalMedicalTestDto
            {
                Id = t.Id,
                ClientId = t.ClientId,
                MedicalData = t.MedicalData,
                CreatedOn = t.CreatedOn,
                CreatedBy = t.CreatedBy
            }).OrderByDescending(t => t.CreatedOn).ToList();
        }

        public async Task DeleteBiochemicalTestAsync(int id)
        {
            await _unitOfWork.Repository<BiochemicalMedicalTest>().DeleteAsync(id);
        }

        #endregion

        #region Drugs/Supplements

        public async Task AddDrugsSupplementAsync(int clientId, DrugsSupplementDto drugDto, int userId)
        {
            var drug = new DrugsSupplement
            {
                ClientId = clientId,
                Drug = drugDto.Drug,
                CreatedBy = userId,
                CreatedOn = DateTime.Now
            };

            await _unitOfWork.Repository<DrugsSupplement>().AddAsync(drug);
        }

        public async Task<List<DrugsSupplementDto>> GetDrugsSupplementsAsync(int clientId)
        {
            var drugs = await _unitOfWork.Repository<DrugsSupplement>()
                .FindAsync(d => d.ClientId == clientId);

            return drugs.Select(d => new DrugsSupplementDto
            {
                Id = d.Id,
                ClientId = d.ClientId,
                Drug = d.Drug,
                CreatedOn = d.CreatedOn,
                CreatedBy = d.CreatedBy
            }).OrderByDescending(d => d.CreatedOn).ToList();
        }

        public async Task DeleteDrugsSupplementAsync(int id)
        {
            await _unitOfWork.Repository<DrugsSupplement>().DeleteAsync(id);
        }

        #endregion

        #region Medical History

        public async Task AddMedicalHistoryAsync(int clientId, MedicalHistoryDto medicalHistoryDto, int userId)
        {
            var medicalHistory = new MedicalHistory
            {
                ClientId = clientId,
                MedicalNote = medicalHistoryDto.MedicalNote,
                CreatedBy = userId,
                CreatedOn = DateTime.Now
            };

            await _unitOfWork.Repository<MedicalHistory>().AddAsync(medicalHistory);
        }

        public async Task<List<MedicalHistoryDto>> GetMedicalHistoriesAsync(int clientId)
        {
            var histories = await _unitOfWork.Repository<MedicalHistory>()
                .FindAsync(m => m.ClientId == clientId);

            return histories.Select(m => new MedicalHistoryDto
            {
                Id = m.Id,
                ClientId = m.ClientId,
                MedicalNote = m.MedicalNote,
                CreatedOn = m.CreatedOn,
                CreatedBy = m.CreatedBy
            }).OrderByDescending(m => m.CreatedOn).ToList();
        }

        public async Task DeleteMedicalHistoryAsync(int id)
        {
            await _unitOfWork.Repository<MedicalHistory>().DeleteAsync(id);
        }

        #endregion
    }
}