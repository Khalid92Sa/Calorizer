using System;
using System.Collections.Generic;

namespace Calorizer.DAL.Models;

public partial class Client
{
    public int Id { get; set; }

    public string FullNameEn { get; set; } = null!;

    public string? FullNameAr { get; set; }

    public string? MobileNumber { get; set; }

    public int GenderId { get; set; }

    public string? Address { get; set; }

    public DateTime DateOfBirth { get; set; }

    public decimal? Weight { get; set; }

    public decimal? Height { get; set; }

    public virtual ICollection<BiochemicalMedicalTest> BiochemicalMedicalTests { get; set; } = new List<BiochemicalMedicalTest>();

    public virtual ICollection<DrugsSupplement> DrugsSupplements { get; set; } = new List<DrugsSupplement>();

    public virtual Lookup Gender { get; set; } = null!;

    public virtual ICollection<MedicalHistory> MedicalHistories { get; set; } = new List<MedicalHistory>();

    public virtual ICollection<WeightHistory> WeightHistories { get; set; } = new List<WeightHistory>();
}
