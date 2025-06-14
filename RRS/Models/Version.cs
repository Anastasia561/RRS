using System;
using System.Collections.Generic;

namespace RRS.Models;

public partial class Version
{
    public int Id { get; set; }

    public int Number { get; set; }

    public DateTime ReleaseDate { get; set; }

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();

    public virtual ICollection<Software> Softwares { get; set; } = new List<Software>();
}
