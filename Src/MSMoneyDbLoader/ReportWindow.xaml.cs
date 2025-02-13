using AAV.Sys.Ext;
using AAV.Sys.Helpers;
using AAV.WPF.Ext;
using AsLink;
using Db.FinDemo.DbModel;
using MF.Common;
using MSMoneyFormatAdapter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace MSMoneyDbLoader
{
  public partial class DbLoaderReportWindow : AAV.WPF.Base.WindowBase
  {
    readonly (bool isFolder, string fileOrFolder) _goal;
    decimal _balns;
    DateTime _dtMin, _dtMax;
    readonly List<string> _files = new List<string>();
    readonly List<TxCoreV2> _txnLst = new List<TxCoreV2>();
    readonly A0DbContext _db = A0DbContext.Create();
    readonly string[] _exts = new[] { ".CSV", ".OFX", ".QFX", ".QIF" };
    /// OFX: td, cibc   
    /// QFX: pcmc   
    /// QIF: amazon only (also pc so: don't use qif from pc!!!) 2017

    public DbLoaderReportWindow(string[] args)
    {
      InitializeComponent();

      _goal = whatAreWeDoingHere(args);

      tbInfo.Text = $"{_goal.fileOrFolder}   -   {(_goal.isFolder ? "Folder" : "File")}";

      Loaded += onLoaded;
      Bpr.BeepOk();
    }
    async void onLoaded(object s, RoutedEventArgs e)
    {
      await Task.Yield();
      await Task.Delay(9);
      await Task.Delay(99);
      await Task.Delay(999);
      if (!_goal.isFolder)
      {
        doFile(_goal.fileOrFolder);
        onCheckDbPendSave();
      }
      else // default check Downloads folder for FIN-files:
      {
        var lst = getFinFiles(_goal.fileOrFolder);
        btn_DoAllDirs.Content = $" _Do InBox Dir ({lst.Count()}) ";
        btn_DoAllDirs.ToolTip = $" There are {lst.Count()} *.?fx files in the {_goal.fileOrFolder} fodler.\r\n\n CLick this button to process(remove) them. ";
        btn_DoAllDirs.FontWeight = (lst.Count() > 0) ? FontWeights.Bold : FontWeights.Normal;

        await doMain(_goal.fileOrFolder);

        App.Synth.SpeakAsync("Review for 600 seconds.");
        for (var i = 6000; i >= 0; i--) { btnCloseWin.Content = $"{i} _X"; await Task.Delay(99); }
      }

      Close();
    }
    protected override void OnClosing(CancelEventArgs e)
    {
      base.OnClosing(e);

      if (DbSaveMsgBox_OldRestoredInDec2023.CheckAskSave(_db) == (int)MsgBoxDbRslt.Cancel)
        e.Cancel = true;
    }
    void onCheckDbPendSave(object s = null, RoutedEventArgs e = null) => tbInfo.Text += $"\r\n{_db.GetDbChangesReport()}";
    void onSaveToDB(object s, RoutedEventArgs e) => tbInfo.Text += $"\r\n{DbSaveMsgBox_OldRestoredInDec2023.CheckAskSave(_db)} rows saved.";
    async void onReadFolder(object s, RoutedEventArgs e)
    {
      try
      {
        ctrlPnl.IsEnabled = false; Bpr.Beep1of2();

        await loadFiles_RefreshGrid_OLD(_goal.fileOrFolder);
      }
      catch (Exception ex) { ex.Log(); }
      finally { ctrlPnl.IsEnabled = true; Bpr.Beep2of2(); }
    }
    void onWindowDrop(object s, DragEventArgs e)
    {
      Bpr.BeepOk();
      if (!e.Data.GetDataPresent(DataFormats.FileDrop)) return;

      foreach (var ff in e.Data.GetData(DataFormats.FileDrop) as string[]) doFileOrFolder(ff);

      onCheckDbPendSave();
    }
    void onRenameFiles(object s, RoutedEventArgs e) => renameFilesToDbDone(_files);
    void onLoadFromDB(object s, RoutedEventArgs e)
    {
      //btnRenameFils.IsEnabled =
      //btnLoadFolder.IsEnabled =
      btnSynchToDB.IsEnabled = false;

      _txnLst.Clear();
      _txnLst.AddRange(_db.TxCoreV2.ToList());
      _txnLst.ForEach(r => r.IsInDb = true);

      refreshGridCalcTotals();
    }
    void onShowChart(object s, RoutedEventArgs e) => new HistoricalChartSet.MainHistChart().Show();
    async void onDoInbox(object s = null, RoutedEventArgs e = null) => await doMain(PreSet.Downloads);
    async Task doMain(string dir) { await doSingleFolder(dir); onCheckDbPendSave(); }
    (bool isFolder, string fileOrFolder) whatAreWeDoingHere(string[] filesOrFoldersFromArgs)
    {
      if (filesOrFoldersFromArgs.Length < 1) return (true, PreSet.Downloads);
      if (File.Exists(filesOrFoldersFromArgs[0])) return (false, filesOrFoldersFromArgs[0]);
      if (Directory.Exists(filesOrFoldersFromArgs[0])) return (true, filesOrFoldersFromArgs[0]);
      throw new InvalidDataException($"AP: {filesOrFoldersFromArgs[0]} is not file nor folder!!!");
    }
    void doFileOrFolder(string fileOrFolder)
    {
      tbInfo.Text += $"\r\nProcessing {fileOrFolder}";

      try
      {
        ctrlPnl.IsEnabled = false;
        Bpr.Beep1of2();
        if (string.IsNullOrEmpty(fileOrFolder)) { tbInfo.Text += "\r\nDrag and Drop an OFX file or folder."; }
        else if (File.Exists(fileOrFolder))
          doFile(fileOrFolder);
        else if (Directory.Exists(fileOrFolder))
          foreach (var file in getFinFiles(fileOrFolder)) doFileOrFolder(file);
        else
          throw new Exception(fileOrFolder + " does not exist: nor file nor folder!!!");

      }
      catch (Exception ex) { ex.Log(); }
      finally { ctrlPnl.IsEnabled = true; Bpr.Beep2of2(); }
    }
    async Task doSingleFolder(string folder)
    {
      Bpr.Beep1of2();

      await _db.TxCoreV2.LoadAsync(); //todo: try test timer .Local.

      foreach (var file in getFinFiles(folder))
      {
        var acntFolder = doFile(file);

        renameFileToDbDone(file, acntFolder);
      }

      Bpr.Beep2of2();

      var rs = DbSaveMsgBox_OldRestoredInDec2023.TrySaveAsk(_db, nameof(doSingleFolder));
      tbInfo.Text += $" {rs,3} rows saved ";
    }
    static IEnumerable<string> getFinFiles(string folder) => Directory.GetFiles(folder, "*.?fx").Union(Directory.GetFiles(folder, "*.qif")).Union(Directory.GetFiles(folder, "*.csv"));
    string doFile(string file)
    {
      Trace.WriteLine(file, "file %$#>");
      string acntFolder;
      TxMoneySrc txMoneySrc;
      foreach (var txFs in MSMoneyFileReader.ReadTxs(_db, file, out acntFolder, out txMoneySrc))
      {
        var txDb = _db.TxCoreV2.Local.FirstOrDefault(db => string.Compare(db.FitId, txFs.FitId) == 0 || db.FitId.Contains(txFs.FitId)) ??
                   _db.TxCoreV2./**/  FirstOrDefault(db => string.Compare(db.FitId, txFs.FitId) == 0 || db.FitId.Contains(txFs.FitId));
        if (txDb == null)
          _db.TxCoreV2.Add(txFs);
        else
        {
          if (txDb.TxMoneySrcId != txFs.TxMoneySrcId)
          {
            if (txDb.TxAmount == txFs.TxAmount) // must be the same txn since ids and $$ match. //todo: must go over all the records and restore those with 'TxMoneySrcId changed from .. to .. on ... ' or re-load the source files.
            {
              txDb.Notes += $" Same FitId & amount: changing TxMoneySrc {txDb.TxMoneySrcId} to {txFs.TxMoneySrcId} on {MSMoneyFileReader._batchTimeNow}   ({txDb.TxMoneySrc.Name} -> {txFs.TxMoneySrc.Name}   <== file-fitid:{file}-{txFs.FitId}).";
              txDb.TxMoneySrcId = txFs.TxMoneySrcId;
              txDb.ModifiedAt = MSMoneyFileReader._batchTimeNow;
              txDb.ModifiedBy = Environment.UserName;
            }
            else
            {
              txFs.Notes += $" Same FitId: {txFs.FitId}  appending  '-{MSMoneyFileReader._batchTimeNow}'   <== file-fitid:{file}-{txFs.FitId}).";
              txFs.FitId += $"-{MSMoneyFileReader._batchTimeNow}";
              _db.TxCoreV2.Add(txFs);
            }

            Trace.WriteLine(txDb.Notes);
          }
#if swap_the_sign_2019_01
          if (txDb.TxAmount == -txFs.TxAmount)
          {
            txDb.Notes += $" +- ";
            txDb.TxAmount = txFs.TxAmount;
            txDb.ModifiedAt = MSMoneyFileReader._batchTimeNow;
            txDb.ModifiedBy = Environment.UserName;
          }
#endif
        }
      }

      insUpdBah(file, txMoneySrc, MSMoneyFileReader.LedgerBal);
      insUpdBah(file, txMoneySrc, MSMoneyFileReader.AvailbBal);

      return acntFolder;
    }
    void insUpdBah(string file, TxMoneySrc txMoneySrc, string balType)
    {
      foreach (var txFs in MSMoneyFileReader.ReadBAH(file, txMoneySrc, balType))
      {
        var txDb = _db.BalAmtHists.FirstOrDefault(r => r.AsOfDate == txFs.AsOfDate && r.BalAmt == txFs.BalAmt && r.BalTpe == balType);
        if (txDb == null)
          _db.BalAmtHists.Add(txFs);
        else
        {
          if (txDb.TxMoneySrcId != txFs.TxMoneySrcId)
          {
            txDb.TxMoneySrcId = txFs.TxMoneySrcId;
            txDb.ModifiedAt = MSMoneyFileReader._batchTimeNow;
            //txDb.ModifiedBy = Environment.UserName;
          }
        }
      }
    }

    async Task loadFiles_RefreshGrid_OLD(string fileOrFolder)
    {
      tbInfo.Text += "\r\nLoading " + fileOrFolder;

      _files.Clear();
      if (File.Exists(fileOrFolder)) { _files.Add(fileOrFolder); /*btnLoadFolder.IsEnabled = true;*/ }
      else if (Directory.Exists(fileOrFolder))
      {
        _files.AddRange(Directory.GetFiles(fileOrFolder, "*.*", SearchOption.TopDirectoryOnly).Where(r => _exts.Contains(System.IO.Path.GetExtension(r).ToUpper())));
        //btnLoadFolder.IsEnabled = false;
      }
      else
        throw new Exception(fileOrFolder + " does not exist: nor file nor folder!!!");

      _txnLst.Clear();
      _files.ForEach(file =>
      {
        _txnLst.AddRange(MSMoneyFileReader.ReadTxs(_db, file, out var acntFolder, out var txSrcId));
      });

      await markEntriesAlreadyInDbAsync(_txnLst);

      refreshGridCalcTotals();
    }
    async Task markEntriesAlreadyInDbAsync(List<TxCoreV2> lst) => await Task.Run(() => markEntriesAlreadyInDb(lst));
    void markEntriesAlreadyInDb(List<TxCoreV2> lst)
    {
      try { foreach (var lr in lst) lr.IsInDb = _db.TxCoreV2.Any(r => string.Compare(r.FitId, lr.FitId) == 0); }
      catch (NotSupportedException ex) { Trace.WriteLine("\nAP: IGNORE: " + ex.Message); }
    }
    void refreshGridCalcTotals()
    {
      ((CollectionViewSource)(FindResource("txCoreV2ViewSource"))).Source = _txnLst.OrderBy(r => r.IsInDb).ThenByDescending(r => r.TxAmount);
      CollectionViewSource.GetDefaultView(txCoreV2DataGrid.ItemsSource).Refresh(); //tu: refresh bound datagrid
      _balns = _txnLst.Sum(r => r.TxAmount);
      _dtMin = _txnLst.Min(r => r.TxDate);
      _dtMax = _txnLst.Max(r => r.TxDate);

      tbInfo.Text += string.Format("\r\n {1:yyyy-MM-dd} ÷ {2:yyyy-MM-dd} = {3:N1} days.    {4}/{5} new/total entries.       {0,12:c} balance.     ", _balns, _dtMin, _dtMax, (_dtMax - _dtMin).TotalDays, _txnLst.Count(r => r.IsInDb == false), _txnLst.Count());

      //btnRenameFils.Content = btnRenameFils.ToolTip = string.Format("_Rename all {0} files to DbDone", _files.Count);
    }
    void renameFilesToDbDone(List<string> files) => files.ForEach(f => renameFileToDbDone(f));
    void onClose(object s, RoutedEventArgs e) => Close();
    void renameFileToDbDone(string webFile, string acntFolder = null)
    {
      if (webFile.Contains(PreSet.DbSynced)) { App.Synth.SpeakAsyncCancelAll(); App.Synth.SpeakAsync($"Archive not moved."); return; }

      var newf = "";
      try
      {
        ctrlPnl.IsEnabled = false;         //too much - no way to reuse debug processed files: if (file.Contains(PreSet.DbSynced)) return;

        Bpr.Beep1of2();
        var fnwe = System.IO.Path.GetFileNameWithoutExtension(webFile);
        var tLst = MSMoneyFileReader.ReadTxs(_db, webFile, out acntFolder, out var txSrcId);
        if (tLst.Count() <= 0)
        {
          MessageBox.Show(acntFolder , "Something wrong.  Aborting the file.", MessageBoxButton.OK, MessageBoxImage.Warning);
          return;
        }

        var dtMin = tLst.Min(r => r.TxDate);
        var dtMax = tLst.Max(r => r.TxDate);
        var newn = string.Format("{0:yyyy-MM-dd} - {1:yyyy-MM-dd}. {3} {2:yyyy-MM-dd}", dtMin, dtMax, DateTime.Today, PreSet.DbSynced);

        newf = webFile.Replace(fnwe, newn);
        if (webFile == newf) { App.Synth.SpeakAsync($"\r\nNew file {Path.GetFileName(newf)} is the same as src. Aborting move."); return; }

        if (acntFolder != null)
        {
          newf = newf.Contains("_In")
            ? newf.Replace("_In", acntFolder)
            : string.Format(PreSet.TrgFormat, acntFolder, newn, Path.GetExtension(webFile));
        }

        while (File.Exists(newf))
          newf = newf.Replace(newn, newn + "~");

        Trace.WriteLine($"\r\nFrom  {Path.GetFileName(webFile)}\r\n  to  {Path.GetFileName(newf)}");
        Trace.WriteLine($"\r\nFrom  {webFile}\r\n  to  {newf}");

        if (!Directory.Exists(Path.GetDirectoryName(newf))) Directory.CreateDirectory(Path.GetDirectoryName(newf));

#if DEBUG
        App.Synth.SpeakAsyncCancelAll();
        App.Synth.SpeakAsync("No files are moved in debug mode.");
#else
        File.Move(webFile, newf);
#endif
        tbInfo.Text += $"\r\n - {Path.GetFileName(webFile),-20} \t=> {acntFolder,10} \\ {Path.GetFileNameWithoutExtension(newf)}   ";

        Bpr.Beep2of2();
      }
      catch (Exception ex) { ex.Pop($"renameFileToDbDone({webFile}, {acntFolder}) ===> {newf}"); }
      finally { ctrlPnl.IsEnabled = true; }
    }
  }
}
