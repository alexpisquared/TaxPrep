namespace Db.FinDemo.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Contribution")]
    public partial class Contribution
    {
        public int Id { get; set; }

        public int FinEngineId { get; set; }

        [Column(TypeName = "date")]
        public DateTime TranDate { get; set; }

        [Column(TypeName = "money")]
        public decimal TranAmount { get; set; }

        [Column(TypeName = "money")]
        public decimal UnitPrice { get; set; }

        public string Notes { get; set; }

        public virtual FinEngine FinEngine { get; set; }
    }
}
