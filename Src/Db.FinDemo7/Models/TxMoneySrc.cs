//using System;
//using System.Collections.Generic;

//namespace Db.FinDemo7.Models;

//public partial class TxMoneySrc
//{
//    public int Id { get; set; }

//    public string Owner { get; set; }

//    public string Fla { get; set; }

//    public string Name { get; set; }

//    public string Notes { get; set; }

//    public decimal IniBalance { get; set; }

//    public DateTime CreatedAt { get; set; }

//    public string Status { get; set; }

//    public virtual ICollection<BalAmtHist> BalAmtHists { get; } = new List<BalAmtHist>();

//    public virtual ICollection<TxCoreV2> TxCoreV2s { get; } = new List<TxCoreV2>();

//    public virtual ICollection<TxCore> TxCores { get; } = new List<TxCore>();
//}

// A copu from  C:\g\TaxPrep\Src\Db.FinDemo7.2024.12\Models\TxMoneySrc.cs:
using System;
using System.Collections.Generic;

namespace Db.FinDemo7.Models;

public partial class TxMoneySrc
{
  public int Id { get; set; }

  public string? Owner { get; set; }

  public string Fla { get; set; } = null!;

  public string Name { get; set; } = null!;

  public string? Notes { get; set; }

  public decimal IniBalance { get; set; }

  public DateTime CreatedAt { get; set; }

  public string? Status { get; set; }

  public virtual ICollection<BalAmtHist> BalAmtHists { get; set; } = new List<BalAmtHist>();

  public virtual ICollection<TxCoreV2> TxCoreV2s { get; set; } = new List<TxCoreV2>();

  public virtual ICollection<TxCore> TxCores { get; set; } = new List<TxCore>();
}
