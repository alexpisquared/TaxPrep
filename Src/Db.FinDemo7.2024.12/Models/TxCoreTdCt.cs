using System;
using System.Collections.Generic;

namespace Db.FinDemo7.Models;

public partial class TxCoreTdCt
{
    public int Id { get; set; }

    public DateTime TxDate { get; set; }

    public string TxDescrn { get; set; } = null!;

    public decimal? TxAmountCrt { get; set; }

    public decimal? TxAmountDbt { get; set; }

    public decimal TxAmountBlnc { get; set; }

    public string AccountId { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public string? Notes { get; set; }
}
