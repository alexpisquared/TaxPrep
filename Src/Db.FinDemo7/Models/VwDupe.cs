using System;
using System.Collections.Generic;

namespace Db.FinDemo7.Models;

public partial class VwDupe
{
    public int Id { get; set; }

    public int TxMoneySrcId { get; set; }

    public int? TxCategoryId { get; set; }

    public string TxCategoryIdTxt { get; set; } = null!;

    public int TxSupplierId { get; set; }

    public int? TxEnvelopeId { get; set; }

    public decimal TxAmount { get; set; }

    public DateTime TxDate { get; set; }

    public string ProductService { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public string? Notes { get; set; }

    public string? History { get; set; }

    public decimal? TxAmountOrg { get; set; }
}
