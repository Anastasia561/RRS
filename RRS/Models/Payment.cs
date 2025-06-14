using System;
using System.Collections.Generic;

namespace RRS.Models;

public partial class Payment
{
    public int Id { get; set; }

    public DateTime Date { get; set; }

    public decimal Amount { get; set; }

    public int ContractId { get; set; }

    public virtual Contract Contract { get; set; } = null!;
}
