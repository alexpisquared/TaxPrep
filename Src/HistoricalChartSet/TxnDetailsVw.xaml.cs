using Db.FinDemo.DbModel;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms.DataVisualization.Charting;

namespace HistoricalChartSet
{
  public partial class TxnDetailsVw : Window
  {
    int _idx;
    List<TxCoreV2> _txs;

    public TxnDetailsVw()
    {
      InitializeComponent(); MouseLeftButtonDown += (s, e) => DragMove();
      //KeyDown += (s, ves) => { switch (ves.Key) { case Key.Escape: Close(); break; } };
      tbNote.Focus();
    }

    public Chart ChartRef { get; internal set; }
    public System.Windows.Forms.DataVisualization.Charting.HitTestResult HTR { get; internal set; }
    public int PointIndex { get; internal set; }
    public TxCoreV2 Txn { set => DataContext = value; }
    public List<TxCoreV2> Txns { set { _txs = value; reShow(_idx = 0); } }
    void reShow(int idx)
    {
      tbIdx.Text = string.Format(" {0} / {1} ", idx + 1, _txs.Count);
      DataContext = _txs[idx];
      btnPrev.IsEnabled = idx > 0;
      btnNext.IsEnabled = idx < _txs.Count - 1;
    }

    void Window_Loaded(object s, RoutedEventArgs e) { }

    void btnPrev_Click(object s, RoutedEventArgs e) => reShow(--_idx);
    void btnNext_Click(object s, RoutedEventArgs e) => reShow(++_idx);
    void btnSave_Click(object s, RoutedEventArgs e) => Close();
    void btnQuit_Click(object s, RoutedEventArgs e) { e.Handled = true; Close(); }
  }
}
