﻿using System;
using System.Collections.Generic;

namespace Db.FinDemo7.Models;

public partial class VwTaxLiqReport2014
{
    public string Group { get; set; } = null!;

    public int? TlNumber { get; set; }

    public string Category { get; set; } = null!;

    public string? PartCalcShow { get; set; }

    public double? TtlExp { get; set; }

    public DateTime? FirstTx { get; set; }

    public DateTime? LastTx { get; set; }

    public int? Qnt { get; set; }
}
