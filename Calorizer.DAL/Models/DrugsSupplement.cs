using System;
using System.Collections.Generic;

namespace Calorizer.DAL.Models;

public partial class DrugsSupplement
{
    public int Id { get; set; }

    public int ClientId { get; set; }

    public string? Drug { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public virtual Client Client { get; set; } = null!;
}
