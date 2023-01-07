using System;
using System.Collections.Generic;

namespace Db.FinDemo7.Models;

public partial class FinEngine
{
    public int Id { get; set; }

    public string DescrName { get; set; }

    public string AccountNumber { get; set; }

    public DateTime? OpenedOn { get; set; }

    public DateTime? ClosedOn { get; set; }

    public DateTime? MaturesOn { get; set; }

    public bool? IsActive { get; set; }

    public string Notes { get; set; }

    public virtual ICollection<Contribution> Contributions { get; } = new List<Contribution>();

    public virtual ICollection<UnitPrice> UnitPrices { get; } = new List<UnitPrice>();
}
