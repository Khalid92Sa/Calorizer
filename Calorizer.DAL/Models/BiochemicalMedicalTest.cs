using System;
using System.Collections.Generic;

namespace Calorizer.DAL.Models;

public partial class BiochemicalMedicalTest
{
    public int Id { get; set; }

    public int ClientId { get; set; }

    public string? MedicalData { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public virtual Client Client { get; set; } = null!;
}
