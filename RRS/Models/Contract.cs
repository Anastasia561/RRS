using System;
using System.Collections.Generic;

namespace RRS.Models;

public partial class Contract
{
    public int Id { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public decimal FinalPrice { get; set; }

    public int UpdateSupport { get; set; }

    public byte IsSigned { get; set; }

    public int ClientId { get; set; }

    public int SoftwareId { get; set; }

    public int VersionId { get; set; }

    public virtual Client Client { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Software Software { get; set; } = null!;

    public virtual Version Version { get; set; } = null!;

    public virtual ICollection<Update> Updates { get; set; } = new List<Update>();
}
