namespace Db.FinDemo.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Vw_Dupes
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TxMoneySrcId { get; set; }

        public int? TxCategoryId { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(50)]
        public string TxCategoryIdTxt { get; set; }

        [Key]
        [Column(Order = 3)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TxSupplierId { get; set; }

        public int? TxEnvelopeId { get; set; }

        [Key]
        [Column(Order = 4, TypeName = "money")]
        public decimal TxAmount { get; set; }

        [Key]
        [Column(Order = 5)]
        public DateTime TxDate { get; set; }

        [Key]
        [Column(Order = 6)]
        [StringLength(200)]
        public string ProductService { get; set; }

        [Key]
        [Column(Order = 7)]
        public DateTime CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public string Notes { get; set; }

        public string History { get; set; }

        [Column(TypeName = "money")]
        public decimal? TxAmountOrg { get; set; }
    }
}
