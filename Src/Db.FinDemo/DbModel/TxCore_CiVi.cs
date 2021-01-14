namespace Db.FinDemo.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TxCore_CiVi
    {
        public int Id { get; set; }

        public DateTime DtPosted { get; set; }

        [Required]
        [StringLength(400)]
        public string Details { get; set; }

        [Column(TypeName = "money")]
        public decimal? Credit { get; set; }

        [Column(TypeName = "money")]
        public decimal? Debit { get; set; }

        [StringLength(400)]
        public string FitId { get; set; }

        [StringLength(400)]
        public string F6 { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public string Notes { get; set; }
    }
}
