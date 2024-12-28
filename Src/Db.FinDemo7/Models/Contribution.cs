using System;
using System.Collections.Generic;

namespace Db.FinDemo7.Models;

public partial class Contribution
{
    public int Id { get; set; }

    public int FinEngineId { get; set; }

    public DateOnly TranDate { get; set; }

    public decimal TranAmount { get; set; }

    public decimal UnitPrice { get; set; }

    public string? Notes { get; set; }

    public virtual FinEngine FinEngine { get; set; } = null!;
}
