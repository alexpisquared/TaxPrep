using System;
using System.Collections.Generic;

namespace Db.FinDemo7.Models;

public partial class VwTxCore
{
    public int? Id { get; set; }

    public int TxMoneySrcId { get; set; }

    public string TxCategoryIdTxt { get; set; }

    public decimal TxAmount { get; set; }

    public DateTime TxDate { get; set; }

    public string TxDetail { get; set; }

    public string ProductService { get; set; }

    public string Notes { get; set; }

    public string MemoPp { get; set; }

    public string History { get; set; }

    public string FitId { get; set; }

    public decimal? CurBalance { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public decimal? TxAmountOrg { get; set; }

    public int? TxCategoryId { get; set; }

    public int? TxSupplierId { get; set; }

    public int? TxEnvelopeId { get; set; }
}
