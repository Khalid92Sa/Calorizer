using System;
using System.Collections.Generic;

namespace Calorizer.DAL.Models;

public partial class LookupCategory
{
    public int Id { get; set; }

    public string NameEn { get; set; } = null!;

    public string NameAr { get; set; } = null!;

    public int Code { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public virtual ICollection<Lookup> Lookups { get; set; } = new List<Lookup>();
}
