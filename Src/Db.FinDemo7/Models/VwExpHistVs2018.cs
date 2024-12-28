using System;
using System.Collections.Generic;

namespace Db.FinDemo7.Models;

public partial class VwExpHistVs2018
{
    public int TaxLiq { get; set; }

    public string ExpenseGroupId { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string IdTxt { get; set; } = null!;

    public string Category { get; set; } = null!;

    public decimal? Exp2010 { get; set; }

    public decimal? Exp2011 { get; set; }

    public decimal? Exp2012 { get; set; }

    public decimal? Exp2013 { get; set; }

    public decimal? Exp2014 { get; set; }

    public decimal? Exp2015 { get; set; }

    public decimal? Exp2016 { get; set; }

    public decimal? Exp2017 { get; set; }

    public decimal? Exp2018 { get; set; }

    public decimal? MaxPrev { get; set; }

    public decimal? PercIncreaseMaxPrevToCurrent { get; set; }
}
