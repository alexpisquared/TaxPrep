namespace Db.FinDemo.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Vw_TaxLiqReport_Mei
    {
        [Key]
        [Column(Order = 0)]
        public string Group { get; set; }

        public int? TL_Number { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(100)]
        public string Category { get; set; }

        [StringLength(28)]
        public string PartCalcShow { get; set; }

        public double? TtlExp { get; set; }

        public DateTime? FirstTx { get; set; }

        public DateTime? LastTx { get; set; }

        public int? Qnt { get; set; }

        [StringLength(3)]
        public string Owner { get; set; }
    }
}
