namespace Db.FinDemo.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TxCoreDetail")]
    public partial class TxCoreDetail
    {
        public int Id { get; set; }

        public int TxCoreId { get; set; }

        public DateTime CreatedAt { get; set; }

        public string Notes { get; set; }

        public virtual TxCore TxCore { get; set; }
    }
}
