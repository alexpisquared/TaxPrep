using System;
using System.Collections.Generic;

namespace Db.FinDemo7.Models;

public partial class VwExp2010Vs2011
{
    public string Name { get; set; } = null!;

    public string Category { get; set; } = null!;

    public decimal? Exp2010 { get; set; }

    public decimal? Exp2011 { get; set; }

    public int? PercIncrease { get; set; }
}
