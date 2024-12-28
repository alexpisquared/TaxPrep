using System;
using System.Collections.Generic;

namespace Db.FinDemo7.Models;

public partial class VwExp20102011Vs2012
{
    public string Name { get; set; } = null!;

    public string Category { get; set; } = null!;

    public decimal? Exp2010 { get; set; }

    public decimal? Exp2011 { get; set; }

    public decimal? Exp2012 { get; set; }

    public int? PercIncrease2010to2011 { get; set; }

    public decimal Avg2010to2011 { get; set; }

    public decimal? PercIncreaseAvg2010to2011to2012 { get; set; }
}
