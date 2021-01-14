namespace Db.FinDemo.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BalAmtHist")]
    public partial class BalAmtHist
    {
        public int Id { get; set; }

        public int TxMoneySrcId { get; set; }

        [Column(TypeName = "money")]
        public decimal BalAmt { get; set; }

        [Required]
        [StringLength(20)]
        public string BalTpe { get; set; }

        public DateTime AsOfDate { get; set; }

        [StringLength(2000)]
        public string Notes { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public virtual TxMoneySrc TxMoneySrc { get; set; }
    }
}
