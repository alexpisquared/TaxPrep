﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace Db.FinDemo.PowerTools.Models;

public partial class TxMoneySrc
{
    public int Id { get; set; }

    public string Owner { get; set; }

    public string Fla { get; set; }

    public string Name { get; set; }

    public string Notes { get; set; }

    public decimal IniBalance { get; set; }

    public DateTime CreatedAt { get; set; }

    public string Status { get; set; }

    public virtual ICollection<TxCoreV2> TxCoreV2 { get; } = new List<TxCoreV2>();
}