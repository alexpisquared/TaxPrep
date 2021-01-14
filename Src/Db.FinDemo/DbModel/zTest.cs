namespace Db.FinDemo.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("zTest")]
    public partial class zTest
    {
        public int Id { get; set; }

        public DateTime? DateTM { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DateDt { get; set; }

        [Column(TypeName = "money")]
        public decimal? Value { get; set; }

        public string Notes { get; set; }
    }
}
