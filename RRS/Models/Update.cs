using System;
using System.Collections.Generic;

namespace RRS.Models;

public partial class Update
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();
}
