namespace Db.FinDemo.DbModel
{
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class TxCoreV2
  {
    [NotMapped]
    public decimal ResultAmt { get; set; }
  }
}
