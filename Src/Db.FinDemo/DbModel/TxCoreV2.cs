namespace Db.FinDemo.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TxCoreV2
    {
        public int Id { get; set; }

        [Required]
        [StringLength(128)]
        public string FitId { get; set; }

        public DateTime TxDate { get; set; }

        [Column(TypeName = "money")]
        public decimal TxAmount { get; set; }

        [Required]
        [StringLength(200)]
        public string TxDetail { get; set; }

        [StringLength(500)]
        public string MemoPP { get; set; }

        [StringLength(2000)]
        public string Notes { get; set; }

        public int TxMoneySrcId { get; set; }

        [Required]
        [StringLength(50)]
        public string TxCategoryIdTxt { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }

        [Column(TypeName = "money")]
        public decimal? CurBalance { get; set; }

        public DateTime? HstTakenAt { get; set; }

        public virtual TxCategory TxCategory { get; set; }

        public virtual TxMoneySrc TxMoneySrc { get; set; }
    }
}
