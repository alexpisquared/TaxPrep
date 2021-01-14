namespace Db.FinDemo.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UnitPrice")]
    public partial class UnitPrice
    {
        public int Id { get; set; }

        public int FinEngineId { get; set; }

        [Column(TypeName = "date")]
        public DateTime Date { get; set; }

        [Column(TypeName = "money")]
        public decimal Value { get; set; }

        public string Notes { get; set; }

        public virtual FinEngine FinEngine { get; set; }
    }
}
