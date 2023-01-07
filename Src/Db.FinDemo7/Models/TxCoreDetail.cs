using System;
using System.Collections.Generic;

namespace Db.FinDemo7.Models;

public partial class TxCoreDetail
{
    public int Id { get; set; }

    public int TxCoreId { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Notes { get; set; }

    public virtual TxCore TxCore { get; set; }
}
