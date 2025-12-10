using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Calorizer.Business.DTOs
{
    public class BiochemicalMedicalTestDto
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string? MedicalData { get; set; }
        public DateTime CreatedOn { get; set; }
        public int CreatedBy { get; set; }
    }
}
