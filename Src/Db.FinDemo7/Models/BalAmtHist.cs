using System;
using System.Collections.Generic;

namespace Db.FinDemo7.Models;

public partial class BalAmtHist
{
    public int Id { get; set; }

    public int TxMoneySrcId { get; set; }

    public decimal BalAmt { get; set; }

    public string BalTpe { get; set; }

    public DateTime AsOfDate { get; set; }

    public string Notes { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public virtual TxMoneySrc TxMoneySrc { get; set; }
}
