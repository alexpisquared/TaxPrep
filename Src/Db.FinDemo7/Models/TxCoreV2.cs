//using System;
//using System.Collections.Generic;

//namespace Db.FinDemo7.Models;

//public partial class TxCoreV2
//{
//    public int Id { get; set; }

//    public string FitId { get; set; }

//    public DateTime TxDate { get; set; }

//    public decimal TxAmount { get; set; }

//    public string TxDetail { get; set; }

//    public string MemoPp { get; set; }

//    public string Notes { get; set; }

//    public int TxMoneySrcId { get; set; }

//    public string TxCategoryIdTxt { get; set; }

//    public DateTime CreatedAt { get; set; }

//    public DateTime? ModifiedAt { get; set; }

//    public decimal? CurBalance { get; set; }

//    public DateTime? HstTakenAt { get; set; }

//    public virtual TxCategory TxCategoryIdTxtNavigation { get; set; }

//    public virtual TxMoneySrc TxMoneySrc { get; set; }
//}

using System;
using System.Collections.Generic;

namespace Db.FinDemo7.Models;

public partial class TxCoreV2
{
  public int Id { get; set; }

  public string FitId { get; set; } = null!;

  public DateTime TxDate { get; set; }

  public decimal TxAmount { get; set; }

  public string TxDetail { get; set; } = null!;

  public string? MemoPp { get; set; }

  public string? Notes { get; set; }

  public int TxMoneySrcId { get; set; }

  public string TxCategoryIdTxt { get; set; } = null!;

  public DateTime CreatedAt { get; set; }

  public DateTime? ModifiedAt { get; set; }

  public decimal? CurBalance { get; set; }

  public DateTime? HstTakenAt { get; set; }

  public virtual TxCategory TxCategoryIdTxtNavigation { get; set; } = null!;

  public virtual TxMoneySrc TxMoneySrc { get; set; } = null!;
}
