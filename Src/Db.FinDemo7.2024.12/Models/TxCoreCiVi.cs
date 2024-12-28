using System;
using System.Collections.Generic;

namespace Db.FinDemo7.Models;

public partial class TxCoreCiVi
{
    public int Id { get; set; }

    public DateTime DtPosted { get; set; }

    public string Details { get; set; } = null!;

    public decimal? Credit { get; set; }

    public decimal? Debit { get; set; }

    public string? FitId { get; set; }

    public string? F6 { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ModifiedAt { get; set; }

    public string? Notes { get; set; }
}
