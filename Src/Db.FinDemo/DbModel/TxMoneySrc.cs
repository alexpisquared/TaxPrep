namespace Db.FinDemo.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TxMoneySrc")]
    public partial class TxMoneySrc
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TxMoneySrc()
        {
            BalAmtHists = new HashSet<BalAmtHist>();
            TxCores = new HashSet<TxCore>();
            TxCoreV2 = new HashSet<TxCoreV2>();
        }

        public int Id { get; set; }

        [StringLength(3)]
        public string Owner { get; set; }

        [Required]
        [StringLength(16)]
        public string Fla { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string Notes { get; set; }

        [Column(TypeName = "money")]
        public decimal IniBalance { get; set; }

        public DateTime CreatedAt { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<BalAmtHist> BalAmtHists { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TxCore> TxCores { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TxCoreV2> TxCoreV2 { get; set; }
    }
}
