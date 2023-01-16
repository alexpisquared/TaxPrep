namespace Db.FinDemo.DbModel
{
  using System;
  using System.Collections.Generic;
  using System.ComponentModel.DataAnnotations;
  using System.ComponentModel.DataAnnotations.Schema;
  using System.Data.Entity.Spatial;

  public partial class Vw_Exp_Hist_vs_Last
  {
    [Key]
    [Column("TaxLiq##", Order = 0)]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int TaxLiq__ { get; set; }

    [Key]
    [Column(Order = 1)]
    [StringLength(50)]
    public string ExpenseGroupId { get; set; }

    [Key]
    [Column(Order = 2)]
    public string Name { get; set; }

    [Key]
    [Column(Order = 3)]
    [StringLength(50)]
    public string IdTxt { get; set; }

    [Key]
    [Column(Order = 4)]
    [StringLength(100)]
    public string Category { get; set; }

    [Column(TypeName = "money")]
    public decimal? Exp2010 { get; set; }

    [Column(TypeName = "money")]
    public decimal? Exp2011 { get; set; }

    [Column(TypeName = "money")]
    public decimal? Exp2012 { get; set; }

    [Column(TypeName = "money")]
    public decimal? Exp2013 { get; set; }

    [Column(TypeName = "money")]
    public decimal? Exp2014 { get; set; }

    [Column(TypeName = "money")]
    public decimal? Exp2015 { get; set; }

    [Column(TypeName = "money")]
    public decimal? Exp2016 { get; set; }

    [Column(TypeName = "money")]
    public decimal? Exp2017 { get; set; }

    [Column(TypeName = "money")]
    public decimal? Exp2018 { get; set; }

    [Column(TypeName = "money")] public decimal? Exp2019 { get; set; }

    [Column(TypeName = "money")] public decimal? Exp2020 { get; set; }
    
    [Column(TypeName = "money")] public decimal? Exp2021 { get; set; }
    [Column(TypeName = "money")] public decimal? Exp2022 { get; set; }

    [Column(TypeName = "money")]
    public decimal? MaxPrev { get; set; }

    [Column(TypeName = "numeric")]
    public decimal? Cur2Max { get; set; }
  }
}
