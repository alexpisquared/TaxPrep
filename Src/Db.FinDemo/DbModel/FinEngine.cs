namespace Db.FinDemo.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("FinEngine")]
    public partial class FinEngine
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FinEngine()
        {
            Contributions = new HashSet<Contribution>();
            UnitPrices = new HashSet<UnitPrice>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(256)]
        public string DescrName { get; set; }

        [Required]
        [StringLength(64)]
        public string AccountNumber { get; set; }

        [Column(TypeName = "date")]
        public DateTime? OpenedOn { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ClosedOn { get; set; }

        [Column(TypeName = "date")]
        public DateTime? MaturesOn { get; set; }

        public bool IsActive { get; set; }

        public string Notes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Contribution> Contributions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UnitPrice> UnitPrices { get; set; }
    }
}
