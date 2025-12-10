using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calorizer.Business.DTOs
{
    public class ClientDto
    {
        public int Id { get; set; }
        public string FullNameEn { get; set; } = string.Empty;
        public string FullNameAr { get; set; } = string.Empty;
        public string? MobileNumber { get; set; }
        public int GenderId { get; set; }
        public string? Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Height { get; set; }

        // Navigation properties for grids
        public List<LookupDto> Genders { get; set; } = new();
        public List<WeightHistoryDto> WeightHistories { get; set; } = new();
        public List<BiochemicalMedicalTestDto> BiochemicalTests { get; set; } = new();
        public List<DrugsSupplementDto> DrugsSupplements { get; set; } = new();
        public List<MedicalHistoryDto> MedicalHistories { get; set; } = new();
    }
}
