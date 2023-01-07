using System;
using System.Collections.Generic;

namespace Db.FinDemo7.Models;

public partial class TxCore
{
    public int Id { get; set; }

    public int TxMoneySrcId { get; set; }

    public int? TxCategoryId { get; set; }

    public string TxCategoryIdTxt { get; set; }

    public int TxSupplierId { get; set; }

    public int? TxEnvelopeId { get; set; }

    public decimal TxAmount { get; set; }

    public DateTime TxDate { get; set; }

    public string ProductService { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public string Notes { get; set; }

    public string History { get; set; }

    public decimal? TxAmountOrg { get; set; }

    public virtual TxCategory TxCategoryIdTxtNavigation { get; set; }

    public virtual ICollection<TxCoreDetail> TxCoreDetails { get; } = new List<TxCoreDetail>();

    public virtual TxEnvelope TxEnvelope { get; set; }

    public virtual TxMoneySrc TxMoneySrc { get; set; }

    public virtual TxSupplier TxSupplier { get; set; }
}
