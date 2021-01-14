namespace Db.FinDemo.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Vw_Exp_2010_2011_vs_2012b
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

        public int? PercIncrease2010to2011 { get; set; }

        [Key]
        [Column(Order = 5, TypeName = "numeric")]
        public decimal Avg2010to2011 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? PercIncreaseAvg2010to2011to2012 { get; set; }

        [Key]
        [Column(Order = 6, TypeName = "money")]
        public decimal Max2010to2011 { get; set; }

        [Column(TypeName = "numeric")]
        public decimal? PercIncreaseMax2010to2011to2012 { get; set; }
    }
}
