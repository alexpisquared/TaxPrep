namespace Db.FinDemo.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TxCore")]
    public partial class TxCore
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TxCore()
        {
            TxCoreDetails = new HashSet<TxCoreDetail>();
        }

        public int Id { get; set; }

        public int TxMoneySrcId { get; set; }

        public int? TxCategoryId { get; set; }

        [Required]
        [StringLength(50)]
        public string TxCategoryIdTxt { get; set; }

        public int TxSupplierId { get; set; }

        public int? TxEnvelopeId { get; set; }

        [Column(TypeName = "money")]
        public decimal TxAmount { get; set; }

        public DateTime TxDate { get; set; }

        [Required]
        [StringLength(200)]
        public string ProductService { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public string Notes { get; set; }

        public string History { get; set; }

        [Column(TypeName = "money")]
        public decimal? TxAmountOrg { get; set; }

        public virtual TxCategory TxCategory { get; set; }

        public virtual TxEnvelope TxEnvelope { get; set; }

        public virtual TxMoneySrc TxMoneySrc { get; set; }

        public virtual TxSupplier TxSupplier { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TxCoreDetail> TxCoreDetails { get; set; }
    }
}
