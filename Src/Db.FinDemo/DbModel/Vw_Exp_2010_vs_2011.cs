namespace Db.FinDemo.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Vw_Exp_2010_vs_2011
    {
        [Key]
        [Column(Order = 0)]
        public string Name { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(100)]
        public string Category { get; set; }

        [Column(TypeName = "money")]
        public decimal? Exp2010 { get; set; }

        [Column(TypeName = "money")]
        public decimal? Exp2011 { get; set; }

        public int? PercIncrease { get; set; }
    }
}
