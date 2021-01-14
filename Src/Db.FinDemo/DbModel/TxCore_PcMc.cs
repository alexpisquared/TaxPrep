namespace Db.FinDemo.DbModel
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TxCore_PcMc
    {
        public int Id { get; set; }

        [Column("Transaction Date")]
        public DateTime Transaction_Date { get; set; }

        [Column("Posting Date")]
        public DateTime Posting_Date { get; set; }

        [Column("Billing Amount", TypeName = "money")]
        public decimal Billing_Amount { get; set; }

        [Required]
        [StringLength(100)]
        public string Merchant { get; set; }

        [Column("Merchant City")]
        [StringLength(100)]
        public string Merchant_City { get; set; }

        [Column("Merchant State")]
        [StringLength(100)]
        public string Merchant_State { get; set; }

        [Column("Merchant Zip")]
        [StringLength(100)]
        public string Merchant_Zip { get; set; }

        [Column("Reference Number")]
        [Required]
        [StringLength(100)]
        public string Reference_Number { get; set; }

        [Column("Debit/Credit Flag")]
        [Required]
        [StringLength(1)]
        public string Debit_Credit_Flag { get; set; }

        [Column("SICMCC Code")]
        [Required]
        [StringLength(10)]
        public string SICMCC_Code { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ModifiedAt { get; set; }

        public string Notes { get; set; }
    }
}
