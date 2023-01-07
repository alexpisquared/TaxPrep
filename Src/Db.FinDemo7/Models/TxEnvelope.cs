using System;
using System.Collections.Generic;

namespace Db.FinDemo7.Models;

public partial class TxEnvelope
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public decimal Total { get; set; }

    public string Name { get; set; }

    public string Note { get; set; }

    public virtual ICollection<TxCore> TxCores { get; } = new List<TxCore>();
}
