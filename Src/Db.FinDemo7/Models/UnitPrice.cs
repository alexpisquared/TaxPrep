using System;
using System.Collections.Generic;

namespace Db.FinDemo7.Models;

public partial class UnitPrice
{
    public int Id { get; set; }

    public int FinEngineId { get; set; }

    public DateTime Date { get; set; }

    public decimal Value { get; set; }

    public string Notes { get; set; }

    public virtual FinEngine FinEngine { get; set; }
}
