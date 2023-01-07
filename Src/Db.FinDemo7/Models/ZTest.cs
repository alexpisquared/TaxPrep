using System;
using System.Collections.Generic;

namespace Db.FinDemo7.Models;

public partial class ZTest
{
    public int Id { get; set; }

    public DateTime? DateTm { get; set; }

    public DateTime? DateDt { get; set; }

    public decimal? Value { get; set; }

    public string Notes { get; set; }
}
