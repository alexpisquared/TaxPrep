using System;
using System.Collections.Generic;

namespace Db.FinDemo7.Models;

public partial class VwTaxLiqReportMei
{
    public string Group { get; set; }

    public int? TlNumber { get; set; }

    public string Category { get; set; }

    public string PartCalcShow { get; set; }

    public double? TtlExp { get; set; }

    public DateTime? FirstTx { get; set; }

    public DateTime? LastTx { get; set; }

    public int? Qnt { get; set; }

    public string Owner { get; set; }
}
