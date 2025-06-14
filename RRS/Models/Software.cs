using System;
using System.Collections.Generic;

namespace RRS.Models;

public partial class Software
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Description { get; set; } = null!;

    public decimal YearCost { get; set; }

    public int CategoryId { get; set; }

    public int CurrentVersionId { get; set; }

    public virtual Category Category { get; set; } = null!;

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();

    public virtual Version CurrentVersion { get; set; } = null!;

    public virtual ICollection<Discount> Discounts { get; set; } = new List<Discount>();
}
