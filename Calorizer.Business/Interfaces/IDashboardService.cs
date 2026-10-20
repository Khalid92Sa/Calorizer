using Calorizer.Business.DTOs;

namespace Calorizer.Business.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardDto> GetDashboardAsync(int recentCount = 5);
    }
}
