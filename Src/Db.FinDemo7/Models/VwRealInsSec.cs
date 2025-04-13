using System;
using System.Collections.Generic;

namespace Db.FinDemo7.Models;

public partial class VwRealInsSec
{
    public int? Year { get; set; }

    public string? Property { get; set; }

    public int? Count { get; set; }

    public decimal? Total { get; set; }

    public decimal? AvgPerMonth { get; set; }
}
