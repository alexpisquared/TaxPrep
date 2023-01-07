using System;
using System.Collections.Generic;

namespace Db.FinDemo7.Models;

public partial class TxCoreAudit
{
    public int Id { get; set; }

    public decimal UpToDateTxAmount { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Notes { get; set; }
}
