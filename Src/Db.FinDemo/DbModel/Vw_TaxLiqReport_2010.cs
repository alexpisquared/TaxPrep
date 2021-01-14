namespace Db.FinDemo.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Vw_TaxLiqReport_2010
    {
        [Key]
        [Column(Order = 0)]
        public string Group { get; set; }

        public int? TL_Number { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(100)]
        public string Category { get; set; }

        [StringLength(19)]
        public string PartCalcShow { get; set; }

        public double? TtlExp { get; set; }

        public int? Qnt { get; set; }

        public DateTime? LastTx { get; set; }
    }
}
