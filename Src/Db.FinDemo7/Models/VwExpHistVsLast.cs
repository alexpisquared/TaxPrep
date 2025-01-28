using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Db.FinDemo7.Models;

[Keyless]
public partial class VwExpHistVsLast
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

    public decimal? Exp2019 { get; set; }

    public decimal? Exp2020 { get; set; }

    public decimal? Exp2021 { get; set; }

    public decimal? Exp2022 { get; set; }

    public decimal? Exp2023 { get; set; }
    
  public decimal? Exp2024 { get; set; } // manually added

    public decimal? MaxPrev { get; set; }

    public decimal? Cur2Max { get; set; }
}
