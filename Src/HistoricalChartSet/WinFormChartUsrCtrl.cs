using AAV.WPF.Ext;
using Db.FinDemo.DbModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace HistoricalChartSet
{
  public partial class WinFormChartUsrCtrl : UserControl
  {
    const string _0now = "$1";
    public WinFormChartUsrCtrl() { InitializeComponent(); Load += onLoad; }


    void onLoad(object s, EventArgs e)
    {
      //var lgd = new string[] { PreSet.__CiVi, PreSet.__PcMc, PreSet.__TdPi, PreSet.__TdCo };      chart1.Series.Clear();      for (int i = 0; i < 4; i++)        chart1.Series.Add(new Series { LegendText = lgd[i], Color = clr[i], ChartType = SeriesChartType.StepLine, XValueType = ChartValueType.DateTime });

      //this.chart1.Titles[0].Text = "wwww";

      ClearAllSeries();
      AddMinSeries();

      chart1.Legends[0].Enabled = true;
      chart1.Legends[0].LegendStyle = LegendStyle.Table;
      chart1.Legends[0].TableStyle = LegendTableStyle.Auto;
      chart1.Legends[0].Docking = Docking.Top;
      chart1.Legends[0].Alignment = StringAlignment.Near;
      //chart1.Legends[0].InsideChartArea = "Default";

      //      chart1.ChartAreas[0].AxisX.ScaleView.Zoom(2, 3);
      chart1.ChartAreas[0].CursorX.IsUserEnabled = true;
      chart1.ChartAreas[0].CursorX.IsUserSelectionEnabled = true;
      chart1.ChartAreas[0].AxisX.ScaleView.Zoomable = true;
      chart1.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = true;

      chart1.ChartAreas[0].CursorY.IsUserEnabled = true;
      chart1.ChartAreas[0].CursorY.IsUserSelectionEnabled = true;
      chart1.ChartAreas[0].AxisY.ScaleView.Zoomable = true;
      chart1.ChartAreas[0].AxisY.ScrollBar.IsPositionedInside = true;

      chart1.ChartAreas[0].AxisX.Maximum =
      chart1.ChartAreas[0].AxisX2.Maximum = DateTime.Today.AddDays(2).ToOADate();
    }

    internal void ClearAllSeries() // => chart1.Series.Clear();
    {
      try
      {
        for (var i = 0; i < chart1.Series.Count; i++)
        {
          var rmv = chart1.Series.FirstOrDefault(r => r.LegendText != _0now);
          if (rmv != null)
            chart1.Series.Remove(rmv);
        }
      }
      catch (Exception ex) { ex.Pop(); }
    }
    internal (decimal calcBal, decimal histBal) AddSeries(string tmsFla, decimal tmsIniBalance, List<TxCoreV2> txn, List<BalAmtHist> bah)
    {
      var txsSrs = new Series { Name = "Txn", LegendText = tmsFla, Color = _srsClr[(1 + 2 * (chart1.Series.Count / 3)) % _srsClr.Length], XValueType = ChartValueType.DateTime, ChartType = SeriesChartType.Column, MarkerStyle = MarkerStyle.None };
      var curSrs = new Series { Name = "Cur", LegendText = tmsFla, Color = _srsClr[(0 + 2 * (chart1.Series.Count / 3)) % _srsClr.Length], XValueType = ChartValueType.DateTime, ChartType = SeriesChartType.StepLine, MarkerStyle = MarkerStyle.Diamond, MarkerSize = 12 };
      var bahSrs = new Series { Name = "BaH", LegendText = tmsFla, Color = _srsClr[(1 + 2 * (chart1.Series.Count / 3)) % _srsClr.Length], XValueType = ChartValueType.DateTime, ChartType = SeriesChartType.StepLine, MarkerStyle = MarkerStyle.None, BorderWidth = 5, BorderDashStyle = ChartDashStyle.Solid };

      var calcBal = tmsIniBalance;
      var histBal = tmsIniBalance;
      foreach (var tx in txn.OrderBy(r => r.TxDate))
      {
        txsSrs.Points.AddXY(tx.TxDate, -tx.TxAmount);
        curSrs.Points.AddXY(tx.TxDate, tx.CurBalance = (calcBal -= tx.TxAmount));
      }

      foreach (var ba in bah.OrderBy(r => r.AsOfDate))
      {
        bahSrs.Points.AddXY(ba.AsOfDate, -ba.BalAmt);
        histBal = -ba.BalAmt;
      }

      chart1.Series.Add(txsSrs); // index: 1
      chart1.Series.Add(curSrs); // index: 2 
      chart1.Series.Add(bahSrs); // index: 3

      return (calcBal, histBal);
    }
    void AddMinSeries() // preventing datetime invalid error on series clear.
    {
      var txsSrs = new Series { LegendText = _0now, MarkerStyle = MarkerStyle.None, ChartType = SeriesChartType.Line };
      txsSrs.Points.AddXY(DateTime.Today, -0);
      txsSrs.Points.AddXY(DateTime.Now, 1);
      chart1.Series.Add(txsSrs);
    }
    internal void RmvSeries(string tmsFla)
    {
      try
      {
        for (var i = 0; i < 3; i++)
        {
          var rmv = chart1.Series.FirstOrDefault(r => r.LegendText == tmsFla);
          if (rmv != null && chart1.Series.Count > 1)
            chart1.Series.Remove(rmv);
        }
      }
      catch (Exception ex) { ex.Pop(); }
    }

    void chart1_GetToolTipText(object s, ToolTipEventArgs e)
    {
      try
      {
        switch (e.HitTestResult.ChartElementType)
        {
          case ChartElementType.DataPoint:
            e.Text = $"{DateTime.FromOADate(e.HitTestResult.Series.Points[e.HitTestResult.PointIndex].XValue):yyyy-MM-dd HH:mm ddd}   -   {e.HitTestResult.Series.Name}" +
              $"\r\nY-Trans-n \t {(chart1.Series.Count > 1 && chart1.Series[1]?.Points?.Count > e.HitTestResult.PointIndex ? chart1.Series[1].Points[e.HitTestResult.PointIndex]?.YValues[0] : null),12:N2}" + // only 1st series, so nogo for multi-series show.
              $"\r\nY-Balance \t {e.HitTestResult.Series.Points[e.HitTestResult.PointIndex].YValues[0],12:N2}" +
              $"";
            Debug.WriteLine(
              $" ** {(chart1.Series[0]?.Points?.Count > e.HitTestResult.PointIndex ? chart1.Series[0]?.Points[e.HitTestResult.PointIndex]?.YValues[0] : null)}" +
              $" ** {(chart1.Series[1]?.Points?.Count > e.HitTestResult.PointIndex ? chart1.Series[1]?.Points[e.HitTestResult.PointIndex]?.YValues[0] : null),12:N2}" +
              $" ** {(chart1.Series[2]?.Points?.Count > e.HitTestResult.PointIndex ? chart1.Series[2]?.Points[e.HitTestResult.PointIndex]?.YValues[0] : null),12:N2}" +
              $" ** ");
            break;
          case ChartElementType.ScrollBarLargeDecrement: e.Text = "A scrollbar large decrement button"; break;
          case ChartElementType.ScrollBarZoomReset: e.Text = "The ZoomReset button of a scrollbar"; break;
          case ChartElementType.Axis: e.Text = e.HitTestResult.Axis.Name; break;
          default: e.Text = e.HitTestResult.ChartElementType.ToString(); break;
        }
      }
      catch (Exception ex) { ex.Pop(); }
    }
    void chart1_MouseMove(object s, MouseEventArgs e)
    {
      var result = chart1.HitTest(e.X, e.Y);

      // If a Data Point or a Legend item is selected.
      if (result.ChartElementType == ChartElementType.DataPoint ||
              result.ChartElementType == ChartElementType.LegendItem)
      {
        Cursor = Cursors.Hand;
      }
      else if (result.ChartElementType != ChartElementType.Nothing &&
                 result.ChartElementType != ChartElementType.PlottingArea)
      {
        Cursor = Cursors.Arrow;
      }
      else
      {
        Cursor = Cursors.Default;
      }
    }
    void chart1_MouseDown(object s, MouseEventArgs e)
    {
      var htr = chart1.HitTest(e.X, e.Y);

      if (htr.ChartElementType == ChartElementType.DataPoint)
      {
        using (var db = A0DbContext.Create())
        {
          var st = DateTime.FromOADate(htr.Series.Points[htr.PointIndex].XValue);
          var txn = db.TxCoreV2.Where(r => r.TxDate == st).ToList();
          if (txn == null)
            MessageBox.Show(this, "Not good: \r\n\tNo txs for this date found.");
          else
          {
            //db.Laps.Load();
            var dlg = new TxnDetailsVw
            {
              ChartRef = chart1,
              PointIndex = htr.PointIndex,
              HTR = htr,
              Txns = txn
            };
            dlg.Show(); // MessageBox.Show(this, "DataPoint.Dialog popup is not ready\r\n\nSelected Element is: " + result.ChartElementType.ToString());
          }
        }
      }
      //else if (result.ChartElementType != ChartElementType.Nothing)
      //{
      //	MessageBox.Show(this, "Selected Element is: " + result.ChartElementType.ToString());
      //}
    }

    #region SeriesColors
    const int scX = 255, sc5 = 128, sc0 = 0;
    readonly Color[] _srsClr = new Color[] {
      Color.FromArgb(255,  sc0, sc0, scX),
      Color.FromArgb( 60,  sc0, sc0, scX),
      Color.FromArgb(255,  sc0, sc5, sc0),
      Color.FromArgb( 60,  sc0, sc5, sc0),
      Color.FromArgb(255,  sc5, sc0, scX),
      Color.FromArgb( 60,  sc5, sc0, scX),
      Color.FromArgb(255,  sc5, sc0, sc0),
      Color.FromArgb( 60,  sc5, sc0, sc0),
      Color.FromArgb(255,  scX, sc0, sc0),
      Color.FromArgb( 60,  scX, sc0, sc0),

      Color.FromArgb(255,  sc0, sc5, sc5),
      Color.FromArgb(255,  sc0, sc5, scX),
      Color.FromArgb(255, sc5, scX, sc5),
      Color.FromArgb(255, sc5,  sc0, sc5),
      Color.FromArgb(255, sc5,  sc0, scX),
      Color.FromArgb(255, sc5, sc5, scX),
      Color.FromArgb(255, sc5, sc5,  sc0),
      Color.FromArgb(255, scX, sc5,  sc0),
      Color.FromArgb(255, sc5, sc5, scX),
      Color.FromArgb(255, sc5, sc5,  sc0),
      Color.FromArgb(255, scX, sc5,  sc0),
    };
    #endregion

  }
}