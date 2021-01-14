using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HistoricalChartSet
{
  public class TxnDtl
  {
    public DateTime TxnTime { get; set; }
    public decimal TxnValue { get; set; }
    public decimal ResultAmt { get; set; }
  }
}
