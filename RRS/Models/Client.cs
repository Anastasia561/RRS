using System;
using System.Collections.Generic;

namespace RRS.Models;

public partial class Client
{
    public int Id { get; set; }

    public string Address { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public virtual Company? Company { get; set; }

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();

    public virtual Individual? Individual { get; set; }
}
