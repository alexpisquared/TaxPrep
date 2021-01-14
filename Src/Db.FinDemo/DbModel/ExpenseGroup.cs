namespace Db.FinDemo.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ExpenseGroup")]
    public partial class ExpenseGroup
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ExpenseGroup()
        {
            TxCategories = new HashSet<TxCategory>();
        }

        [StringLength(50)]
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Note { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TxCategory> TxCategories { get; set; }
    }
}
