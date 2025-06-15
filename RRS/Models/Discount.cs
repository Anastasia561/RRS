using System;
using System.Collections.Generic;

namespace RRS.Models;

public partial class Discount
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public byte IsUpfront { get; set; }

    public int Value { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public virtual ICollection<Software> Softwares { get; set; } = new List<Software>();
}