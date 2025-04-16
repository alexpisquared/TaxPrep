namespace Db.FinDemo7.Models;

using System.ComponentModel.DataAnnotations.Schema;

public partial class TxCoreV2
{
  [NotMapped]
  public bool IsInDb { get; set; }
  [NotMapped]
  public string SrcFile { get; set; }

  public override string ToString() => $"{TxDate:yyyy-MM-dd HH:mm}{TxAmount,10:N2}{FitId,-40}{TxDetail,-48}{MemoPp,-48}  \"{Notes}\"";
}
