using System;
using System.Collections.Generic;

namespace Db.FinDemo7.Models;

public partial class TxCategory
{
    public int Id { get; set; }

    public string IdTxt { get; set; }

    public string ExpGroupId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Name { get; set; }

    public string Notes { get; set; }

    public int? TlNumber { get; set; }

    public double DeductiblePercentage { get; set; }

    public virtual ExpenseGroup ExpGroup { get; set; }

    public virtual ICollection<TxCoreV2> TxCoreV2s { get; } = new List<TxCoreV2>();

    public virtual ICollection<TxCore> TxCores { get; } = new List<TxCore>();
}
