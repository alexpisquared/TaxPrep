using System;
using System.Collections.Generic;

namespace Db.FinDemo7.Models;

public partial class FinEngine
{
    public int Id { get; set; }

    public string DescrName { get; set; } = null!;

    public string AccountNumber { get; set; } = null!;

    public DateOnly? OpenedOn { get; set; }

    public DateOnly? ClosedOn { get; set; }

    public DateOnly? MaturesOn { get; set; }

    public bool IsActive { get; set; }

    public string? Notes { get; set; }

    public virtual ICollection<Contribution> Contributions { get; set; } = new List<Contribution>();

    public virtual ICollection<UnitPrice> UnitPrices { get; set; } = new List<UnitPrice>();
}
