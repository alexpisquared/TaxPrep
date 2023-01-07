using System;
using System.Collections.Generic;

namespace Db.FinDemo7.Models;

public partial class VwExpHistVs2018Zoe
{
    public int TaxLiq { get; set; }

    public string ExpenseGroupId { get; set; }

    public string Name { get; set; }

    public string IdTxt { get; set; }

    public string Category { get; set; }

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
