using System;
using System.Collections.Generic;

namespace Db.FinDemo7.Models;

public partial class VwTaxLiqReport2011
{
    public string Group { get; set; } = null!;

    public int? TlNumber { get; set; }

    public string Category { get; set; } = null!;

    public string? PartCalcShow { get; set; }

    public double? TtlExp { get; set; }

    public int? Qnt { get; set; }

    public DateTime? LastTx { get; set; }
}
