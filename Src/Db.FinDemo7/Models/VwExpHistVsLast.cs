using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Db.FinDemo7.Models;

public partial class VwExpHistVsLast
{
    public int TaxLiq { get; set; }

    public string ExpenseGroupId { get; set; }

    public string Name { get; set; }

  [Key] //tu: patch vw-no-kwy 2/2   ...because it does not have a primary key   
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

    public decimal? Exp2019 { get; set; }

    public decimal? Exp2020 { get; set; }

    public decimal? Exp2021 { get; set; }

    public decimal? MaxPrev { get; set; }

    public decimal? Cur2Max { get; set; }
}
