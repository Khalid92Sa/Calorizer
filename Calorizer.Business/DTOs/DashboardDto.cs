namespace Calorizer.Business.DTOs
{
    public class DashboardDto
    {
        // KPI cards
        public int TotalClients { get; set; }
        public int NewClientsThisMonth { get; set; }
        public int UpdatedClientsThisMonth { get; set; }
        public int CalculationNotesCount { get; set; }

        // Breakdowns
        public List<DashboardCountItem> ClientsByGender { get; set; } = new();
        public int BmiUnderweight { get; set; }   // < 18.5
        public int BmiNormal { get; set; }        // 18.5 – 24.5
        public int BmiOverweight { get; set; }    // > 24.5
        public int BmiUnknown { get; set; }       // missing height / weight

        // Short lists
        public List<DashboardClientItem> RecentlyAdded { get; set; } = new();
        public List<DashboardClientItem> RecentlyUpdated { get; set; } = new();
    }

    public class DashboardCountItem
    {
        public string NameEn { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class DashboardClientItem
    {
        public int Id { get; set; }
        public string FullNameEn { get; set; } = string.Empty;
        public string? FullNameAr { get; set; }
        public string? MobileNumber { get; set; }
        public decimal? Bmi { get; set; }
        public DateTime? Date { get; set; }
    }
}
