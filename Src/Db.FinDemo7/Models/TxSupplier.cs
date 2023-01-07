using System;
using System.Collections.Generic;

namespace Db.FinDemo7.Models;

public partial class TxSupplier
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Name { get; set; }

    public string Notes { get; set; }

    public virtual ICollection<TxCore> TxCores { get; } = new List<TxCore>();
}
