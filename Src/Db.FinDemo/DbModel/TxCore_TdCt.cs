namespace Db.FinDemo.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TxCore_TdCt
    {
        public int Id { get; set; }

        public DateTime TxDate { get; set; }

        [Required]
        [StringLength(400)]
        public string TxDescrn { get; set; }

        [Column(TypeName = "money")]
        public decimal? TxAmountCrt { get; set; }

        [Column(TypeName = "money")]
        public decimal? TxAmountDbt { get; set; }

        [Column(TypeName = "money")]
        public decimal TxAmountBlnc { get; set; }

        [Required]
        [StringLength(4)]
        public string AccountId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public string Notes { get; set; }
    }
}
