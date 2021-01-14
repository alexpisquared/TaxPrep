namespace Db.FinDemo.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TxEnvelope")]
    public partial class TxEnvelope
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TxEnvelope()
        {
            TxCores = new HashSet<TxCore>();
        }

        public int Id { get; set; }

        public DateTime CreatedAt { get; set; }

        [Column(TypeName = "money")]
        public decimal Total { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string Note { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TxCore> TxCores { get; set; }
    }
}
