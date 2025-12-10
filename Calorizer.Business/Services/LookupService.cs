using Calorizer.Business.DTOs;
using Calorizer.Business.Enums;
using Calorizer.Business.Interfaces;
using Calorizer.DAL.Models;
using Calorizer.DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Calorizer.Business.Services
{
    public class LookupService : ILookupService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LookupService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<LookupDto>> GetLookupItems(LookupTypes lookupType)
        {
            var categoryCode = (int)lookupType;
            return await GetLookupItemsByCategoryId(categoryCode);
        }

        public async Task<List<LookupDto>> GetLookupItemsByCategoryId(int categoryCode)
        {
            var lookups = await _unitOfWork.Repository<Lookup>()
                .FindAsync(l => l.Category.Code == categoryCode);

            return lookups.Select(l => new LookupDto
            {
                Id = l.Id,
                NameEn = l.NameEn,
                NameAr = l.NameAr,
                Code = l.Code
            }).ToList();
        }

        public async Task<LookupDto?> GetLookupById(int id)
        {
            var lookup = await _unitOfWork.Repository<Lookup>().GetByIdAsync(id);

            if (lookup == null)
                return null;

            return new LookupDto
            {
                Id = lookup.Id,
                NameEn = lookup.NameEn,
                NameAr = lookup.NameAr,
                Code = lookup.Code
            };
        }
    }
}