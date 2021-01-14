namespace Db.FinDemo.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TxSupplier")]
    public partial class TxSupplier
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TxSupplier()
        {
            TxCores = new HashSet<TxCore>();
        }

        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string Notes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TxCore> TxCores { get; set; }
    }
}
