using System;
using System.Collections.Generic;

namespace Calorizer.DAL.Models;

public partial class WeightHistory
{
    public int Id { get; set; }

    public int ClientId { get; set; }

    public decimal? Weight { get; set; }

    public decimal? Height { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedOn { get; set; }

    public virtual Client Client { get; set; } = null!;
}
