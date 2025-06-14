using System;
using System.Collections.Generic;

namespace RRS.Models;

public partial class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Software> Softwares { get; set; } = new List<Software>();
}
