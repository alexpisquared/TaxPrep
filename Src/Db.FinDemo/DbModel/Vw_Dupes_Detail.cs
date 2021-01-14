namespace Db.FinDemo.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Vw_Dupes_Detail
    {
        [Key]
        [Column(Order = 0, TypeName = "money")]
        public decimal TxAmount { get; set; }

        [Key]
        [Column(Order = 1)]
        public DateTime TxDate { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(50)]
        public string TxCategoryIdTxt { get; set; }

        [Key]
        [Column(Order = 3)]
        [StringLength(200)]
        public string ProductService { get; set; }

        public string Notes { get; set; }

        [Key]
        [Column(Order = 4)]
        [StringLength(100)]
        public string Supplier { get; set; }

        [Key]
        [Column(Order = 5)]
        [StringLength(100)]
        public string MoneySrc { get; set; }

        public string History { get; set; }

        [Key]
        [Column(Order = 6)]
        public DateTime CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }
    }
}
