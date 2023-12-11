using AAV.Sys.Ext;
using AAV.Sys.Helpers;
using AsLink;
using Db.FinDemo.DbModel;
using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace HistoricalChartSet
{
  public partial class MainHistChart : AAV.WPF.Base.WindowBase
  {
    A0DbContext _db; int _tmsId; string _tmsFla; decimal _balDelta;

    public MainHistChart() { InitializeComponent(); Loaded += onLoaded; }

    async void onLoaded(object s, RoutedEventArgs e) => await reload();
    void onSelChanged(object s, SelectionChangedEventArgs e)
    {
      try
      {
        _tmsId = -1; _tmsFla = "...";
        foreach (Tms tms in e.RemovedItems) { chartuc.RmvSeries(tms.Fla); }
        foreach (Tms tms in e.AddedItems) { addSeries(_tmsId = tms.Id, _tmsFla = tms.Fla, tms.IniBalance); }
      }
      catch (Exception ex) { ex.Log(); }
    }
    void onShowAll(object s, RoutedEventArgs e) { chartuc.ClearAllSeries(); foreach (TxMoneySrc tms in lbMnySrc_Flas.ItemsSource) addSeries(tms.Id, tms.Fla, tms.IniBalance); }
    void onClearAll(object s, RoutedEventArgs e) => chartuc.ClearAllSeries();
    async void onBalanceToDb(object s, RoutedEventArgs e)
    {
      try
      {
        bBalance.Visibility = Visibility.Collapsed;
        await Task.Delay(9);

        using (var db = A0DbContext.Create())
        {
          var tmsdb = db.TxMoneySrcs.FirstOrDefault(r => r.Id == _tmsId);
          if (tmsdb != null)
          {
            tmsdb.IniBalance += _balDelta;
            var rowsSaved = DbSaveMsgBox_OldRestoredInDec2023.CheckAskSave(db);
            tBalance.Text = $"{rowsSaved} row saved.";
            await reload();
            chartuc.RmvSeries(_tmsFla);
            addSeries(tmsdb.Id, tmsdb.Fla, tmsdb.IniBalance);
          }
        }
      }
      finally { /*bBalance.Visibility = Visibility.Visible;*/ }
    }
    void onCopy(object s, System.Windows.Input.MouseButtonEventArgs e) => Clipboard.SetText(((TextBlock)s).Text);

    async Task reload()
    {
      _db = A0DbContext.Create();
      tbComp.Text = VerHelper.CurVerStr(A0DbContext.SolutionCfg);

      try
      {
        await _db.TxMoneySrcs.LoadAsync();
        await _db.BalAmtHists.LoadAsync();
        await _db.TxCoreV2.LoadAsync();     //full history is here: await _db.Vw_TxCore.LoadAsync(); - is it worth it/useful

        var groupByMnySrc = _db.TxCoreV2.Local.GroupBy(c => new { c.TxMoneySrcId }).Select(g => new { g.Key.TxMoneySrcId, Count = g.Count(), LastTx = g.Max(r => r.TxDate) }).ToList();

        //var usedMnySrcList = _db.TxCoreV2.Local.Select(r => r.TxMoneySrcId).Distinct().ToList();        //lbMnySrc_Flas.ItemsSource = _db.TxMoneySrcs.Local.Where(r => usedMnySrcList.Contains(r.Id)).OrderBy(r => r.Fla);
        lbMnySrc_Flas.ItemsSource = _db.TxMoneySrcs.Local.
          Join(groupByMnySrc, s => s.Id, c => c.TxMoneySrcId, (s, c) => new { s, c }).
          Select(r => new Tms(r.s.Id, r.s.Fla, r.s.Name,
          r.s.IniBalance,
          r.c.LastTx,
          $"{r.s.Notes}\r\nCount: \t {r.c.Count}\r\nLast: \t {r.c.LastTx:yyy-MM-dd}"
          ));

        lbMnySrc_Flas.Focus();
      }
      catch (Exception ex) { ex.Log(); }
    }
    void addSeries(int tmsId, string tmsFla, decimal tmsIniBal)
    {
      try
      {
        var txs = _db.TxCoreV2.Local.Where(r => r.TxMoneySrcId == tmsId).ToList();
        var bah = _db.BalAmtHists.Local.Where(r => r.TxMoneySrcId == tmsId && r.BalTpe == "LEDGERBAL").ToList();
        var (calcBal, histBal) = chartuc.AddSeries(tmsFla, tmsIniBal, txs, bah);
        _balDelta = -(calcBal - histBal);
        bBalance.Visibility = _balDelta != 0 ? Visibility.Visible : Visibility.Collapsed;
        bBalance.Content = $"_Rebalance '{tmsFla}'";

        tbk_____.Text = _balDelta == 0 ? $"{tmsFla} is balanced" : $"{histBal:N0}  -  {calcBal:N0}  =  ";
        tBalance.Text = _balDelta == 0 ? $"" : $"{tmsFla} is  off by {_balDelta:N4}";
        tbk_____.Foreground = tBalance.Foreground = new SolidColorBrush(_balDelta == 0 ? Colors.Green : Colors.Red);

        tTxRange.Text =
          $"txn: {txs.Min(r => r.TxDate):yyy·MMM·dd} ÷ {txs.Max(r => r.TxDate):yyy·MMM·dd}       " +
          $"bah: {(bah.Count > 0 ? bah.Min(r => r.AsOfDate) : DateTime.MinValue):yyy·MMM·dd} ÷ {(bah.Count > 0 ? bah.Max(r => r.AsOfDate) : DateTime.MinValue):yyy·MMM·dd}";
      }
      catch (Exception ex) { ex.Log(); }
    }

    class Tms
    {
      public Tms(int id, string fla, string name, decimal inibalance, DateTime lastTx, string notes)
      {
        Id = id;
        Fla = fla;
        Name = name;
        IniBalance = inibalance;
        LastTx = lastTx;
        Notes = notes;
      }

      public int Id { get; set; }
      public string Fla { get; set; }
      public string Name { get; set; }
      public decimal IniBalance { get; set; }
      public DateTime LastTx { get; set; }
      public string Notes { get; set; }
    }
  }
}
