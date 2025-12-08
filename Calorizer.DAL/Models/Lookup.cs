using System;
using System.Collections.Generic;

namespace Calorizer.DAL.Models;

public partial class Lookup
{
    public int Id { get; set; }

    public string NameEn { get; set; } = null!;

    public string NameAr { get; set; } = null!;

    public string? Code { get; set; }

    public int CategoryId { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public int? ModifiedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public virtual LookupCategory Category { get; set; } = null!;

    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();
}
