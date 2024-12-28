using System;
using System.Collections.Generic;

namespace Db.FinDemo7.Models;

public partial class TxCategory
{
    public int Id { get; set; }

    public string IdTxt { get; set; } = null!;

    public string ExpGroupId { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public string Name { get; set; } = null!;

    public string? Notes { get; set; }

    public int? TlNumber { get; set; }

    public double DeductiblePercentage { get; set; }

    public virtual ExpenseGroup ExpGroup { get; set; } = null!;

    public virtual ICollection<TxCoreV2> TxCoreV2s { get; set; } = new List<TxCoreV2>();

    public virtual ICollection<TxCore> TxCores { get; set; } = new List<TxCore>();
}
