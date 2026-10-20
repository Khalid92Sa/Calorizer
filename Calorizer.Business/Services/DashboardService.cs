using Calorizer.Business.DTOs;
using Calorizer.Business.Enums;
using Calorizer.Business.Interfaces;
using Calorizer.DAL.Models;
using Calorizer.DAL.Repositories;

namespace Calorizer.Business.Services
{
    public class DashboardService : IDashboardService
    {
        private const decimal BmiLow = 18.5m;
        private const decimal BmiHigh = 24.5m;

        private readonly IUnitOfWork _unitOfWork;
        private readonly ILookupService _lookupService;

        public DashboardService(IUnitOfWork unitOfWork, ILookupService lookupService)
        {
            _unitOfWork = unitOfWork;
            _lookupService = lookupService;
        }

        public async Task<DashboardDto> GetDashboardAsync(int recentCount = 5)
        {
            var clients = (await _unitOfWork.Repository<Client>().GetAllAsync()).ToList();
            var genders = await _lookupService.GetLookupItems(LookupTypes.Gender);
            var monthStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            var dashboard = new DashboardDto
            {
                TotalClients = clients.Count,
                NewClientsThisMonth = clients.Count(c => c.CreatedOn >= monthStart),
                UpdatedClientsThisMonth = clients.Count(c => c.ModifiedOn >= monthStart),
                CalculationNotesCount = await _unitOfWork.Repository<CalculationNote>().CountAsync(),

                ClientsByGender = genders
                    .Select(g => new DashboardCountItem
                    {
                        NameEn = g.NameEn,
                        NameAr = g.NameAr,
                        Count = clients.Count(c => c.GenderId == g.Id)
                    })
                    .ToList(),

                RecentlyAdded = clients
                    .Where(c => c.CreatedOn.HasValue)
                    .OrderByDescending(c => c.CreatedOn)
                    .Take(recentCount)
                    .Select(c => ToItem(c, c.CreatedOn))
                    .ToList(),

                RecentlyUpdated = clients
                    .Where(c => c.ModifiedOn.HasValue)
                    .OrderByDescending(c => c.ModifiedOn)
                    .Take(recentCount)
                    .Select(c => ToItem(c, c.ModifiedOn))
                    .ToList()
            };

            foreach (var client in clients)
            {
                var bmi = CalculateBmi(client.Weight, client.Height);
                if (bmi == null) dashboard.BmiUnknown++;
                else if (bmi < BmiLow) dashboard.BmiUnderweight++;
                else if (bmi <= BmiHigh) dashboard.BmiNormal++;
                else dashboard.BmiOverweight++;
            }

            return dashboard;
        }

        private static DashboardClientItem ToItem(Client c, DateTime? date) => new()
        {
            Id = c.Id,
            FullNameEn = c.FullNameEn,
            FullNameAr = c.FullNameAr,
            MobileNumber = c.MobileNumber,
            Bmi = CalculateBmi(c.Weight, c.Height),
            Date = date
        };

        // BMI = Weight (kg) / (Height in m)^2 — Height stored in cm
        private static decimal? CalculateBmi(decimal? weight, decimal? heightCm)
        {
            if (weight is not > 0m || heightCm is not > 0m) return null;
            var heightM = heightCm.Value / 100m;
            return Math.Round(weight.Value / (heightM * heightM), 1);
        }
    }
}
