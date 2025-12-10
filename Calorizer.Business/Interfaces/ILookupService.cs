using Calorizer.Business.DTOs;
using Calorizer.Business.Enums;

namespace Calorizer.Business.Interfaces
{
    public interface ILookupService
    {
        Task<List<LookupDto>> GetLookupItems(LookupTypes lookupType);
        Task<List<LookupDto>> GetLookupItemsByCategoryId(int categoryId);
        Task<LookupDto?> GetLookupById(int id);
    }
}