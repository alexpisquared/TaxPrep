namespace Db.FinDemo.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TxCoreAudit")]
    public partial class TxCoreAudit
    {
        public int Id { get; set; }

        [Column(TypeName = "money")]
        public decimal UpToDateTxAmount { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Notes { get; set; }
    }
}
