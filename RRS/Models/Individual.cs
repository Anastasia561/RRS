using System;
using System.Collections.Generic;

namespace RRS.Models;

public partial class Individual
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string Pesel { get; set; } = null!;

    public byte IsRemoved { get; set; }

    public virtual Client IdNavigation { get; set; } = null!;
}
