namespace Db.FinDemo.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TxCategory")]
    public partial class TxCategory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TxCategory()
        {
            TxCores = new HashSet<TxCore>();
            TxCoreV2 = new HashSet<TxCoreV2>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Key]
        [StringLength(50)]
        public string IdTxt { get; set; }

        [Required]
        [StringLength(50)]
        public string ExpGroupId { get; set; }

        public DateTime CreatedAt { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string Notes { get; set; }

        public int? TL_Number { get; set; }

        public double DeductiblePercentage { get; set; }

        public virtual ExpenseGroup ExpenseGroup { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TxCore> TxCores { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TxCoreV2> TxCoreV2 { get; set; }
    }
}
