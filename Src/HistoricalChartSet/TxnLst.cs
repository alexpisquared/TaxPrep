using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HistoricalChartSet
{
  public class TxnLst
  {
    List<TxnDtl> pcTxnDtl = new List<TxnDtl>();

    public List<TxnDtl> PcTxnDtl { get { return pcTxnDtl; } set { pcTxnDtl = value; } }

  }
}
