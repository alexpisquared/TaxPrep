﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Db.FinDemo.PowerTools.Models;

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

    public virtual ICollection<TxCoreV2> TxCoreV2 { get; } = new List<TxCoreV2>();
}