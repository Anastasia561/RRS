using System;
using System.Collections.Generic;

namespace RRS.Models;

public partial class Company
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Krs { get; set; } = null!;

    public virtual Client IdNavigation { get; set; } = null!;
}
