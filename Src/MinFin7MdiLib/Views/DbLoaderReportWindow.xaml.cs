using Microsoft.Win32;
using System.IO;

namespace MinFin7MdiLib.Views;
public partial class DbLoaderReportWindow
{
  readonly (bool isFolder, string fileOrFolder) _goal;
  decimal _balns;
  DateTime _dtMin, _dtMax;
  readonly List<string> _files = [];
  readonly List<TxCoreV2> _txnLst = [];
  readonly string[] _exts = new[] { ".CSV", ".OFX", ".QFX", ".QIF" };
  /// OFX: td, cibc   
  /// QFX: pcmc   
  /// QIF: amazon only (also pc so: don't use qif from pc!!!) 2017

  readonly FinDemoContext _dbx;
  readonly DateTime _yrStart2004 = new(2004, 1, 1), _now = DateTime.Now;
  readonly ObservableCollection<TxCoreV2> _txCoreItems = [];
  readonly ObservableCollection<TxCategory> _txCategories = [];
  readonly CollectionViewSource _txCategoryCmBxVwSrc, _txCategoryDGrdVwSrc, _txCoreV2_Root_VwSrc;
  readonly int _trgTaxYr = DateTime.Today.Year - 1;
  readonly ILogger _lgr;
  readonly IBpr _bpr;
  private readonly SpeechSynth _sth;
  readonly string? _txCatgry, _loadedCatgry = "?", _choiceAbove, _choiceBelow;
  readonly decimal _selectTtl = 0;
  readonly bool _loaded = false;
  readonly int? _cutOffYr = null;

  public DbLoaderReportWindow(ILogger lgr, IBpr bpr, SpeechSynth sth, FinDemoContext dbx, string[] args)
  {
    InitializeComponent();

    _lgr = lgr;
    _bpr = bpr;
    _sth = sth;
    _dbx = dbx;

    _goal = whatAreWeDoingHere(args);

    tbInfo.Text = $"{_goal.fileOrFolder}   -   {(_goal.isFolder ? "Folder" : "File")}";

    _bpr.Tick();
  }
  async void onLoaded(object s, RoutedEventArgs e)
  {
    await Task.Delay(99);
    if (!_goal.isFolder)
    {
      _ = doFile(_goal.fileOrFolder);
      onCheckDbPendSave();
    }
    else // default check Downloads folder for FIN-files:
    {
      var lst = getFinFiles(_goal.fileOrFolder);
      btn_DoAllDirs.Content = $" _Do InBox Dir ({lst.Count()}) ";
      btn_DoAllDirs.ToolTip = $" There are {lst.Count()} *.?fx files in the {_goal.fileOrFolder} fodler.\r\n\n CLick this button to process(remove) them. ";
      btn_DoAllDirs.FontWeight = (lst.Count() > 0) ? FontWeights.Bold : FontWeights.Normal;

      await doMain(_goal.fileOrFolder);

      await _sth.SpeakAsync("Will self close in 600 seconds.");
      for (var i = 6000; i >= 0; i--) { btnCloseWin.Content = $"{i} _X"; await Task.Delay(99); }
    }

    Close();
  }
  protected override void OnClosing(CancelEventArgs e)
  {
    base.OnClosing(e);

    if (DbSaveMsgBox.CheckAskSave(_dbx) == (int)MsgBoxDbRslt.Cancel)
      e.Cancel = true;
  }
  void onCheckDbPendSave(object? s = null, RoutedEventArgs? e = null) => tbInfo.Text += $"\r\n{_dbx.GetDbChangesReport()}";
  void onSaveToDB(object s, RoutedEventArgs e) => tbInfo.Text += $"\r\n{DbSaveMsgBox.CheckAskSave(_dbx)} rows saved.";
  async void onReadFolder(object s, RoutedEventArgs e)
  {
    try
    {
      ctrlPnl.IsEnabled = false; _bpr.Beep1of2();

      await loadFiles_RefreshGrid_OLD(_goal.fileOrFolder);
    }
    catch (Exception ex) { _ = ex.Log(); }
    finally { ctrlPnl.IsEnabled = true; _bpr.Beep2of2(); }
  }
  void onWindowDrop(object s, DragEventArgs e)
  {
    _bpr.Click();
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
    _txnLst.AddRange(_dbx.TxCoreV2s.ToList());
    _txnLst.ForEach(r => r.IsInDb = true);

    refreshGridCalcTotals();
  }
  //void onShowChart(object s, RoutedEventArgs e) => new HistoricalChartSet.MainHistChart().Show();
  async void onDoInbox(object? s = null, RoutedEventArgs? e = null) => await doMain(PreSet.Downloads);
  async Task doMain(string dir) { await doSingleFolder(dir); onCheckDbPendSave(); }
  (bool isFolder, string fileOrFolder) whatAreWeDoingHere(string[] filesOrFoldersFromArgs)
  {
    if (filesOrFoldersFromArgs.Length < 1) return (true, PreSet.Downloads);
    if (File.Exists(filesOrFoldersFromArgs[0])) return (false, filesOrFoldersFromArgs[0]);
    return Directory.Exists(filesOrFoldersFromArgs[0])
      ? ((bool isFolder, string fileOrFolder))(true, filesOrFoldersFromArgs[0])
      : throw new InvalidDataException($"AP: {filesOrFoldersFromArgs[0]} is not file nor folder!!!");
  }
  void doFileOrFolder(string fileOrFolder)
  {
    tbInfo.Text += $"\r\nProcessing {fileOrFolder}";

    try
    {
      ctrlPnl.IsEnabled = false;
      _bpr.Beep1of2();
      if (string.IsNullOrEmpty(fileOrFolder)) { tbInfo.Text += "\r\nDrag and Drop an OFX file or folder."; }
      else if (File.Exists(fileOrFolder))
        _ = doFile(fileOrFolder);
      else if (Directory.Exists(fileOrFolder))
        foreach (var file in getFinFiles(fileOrFolder)) doFileOrFolder(file);
      else
        throw new Exception(fileOrFolder + " does not exist: nor file nor folder!!!");

    }
    catch (Exception ex) { _ = ex.Log(); }
    finally { ctrlPnl.IsEnabled = true; _bpr.Beep2of2(); }
  }
  async Task doSingleFolder(string folder)
  {
    _bpr.Beep1of2();

    await _dbx.TxCoreV2s.LoadAsync(); //todo: try test timer .Local.

    foreach (var file in getFinFiles(folder))
    {
      WriteLine(file); // continue;

      var acntFolder = doFile(file);

      renameFileToDbDone(file, acntFolder);
    }

    _bpr.Beep2of2();

    var rs = DbSaveMsgBox.CheckAskSave(_dbx);  // .TrySaveAsk(_dbx, nameof(doSingleFolder));
    tbInfo.Text += $" {rs,3} rows saved ";
  }
  static IEnumerable<string> getFinFiles(string folder) => Directory.GetFiles(folder, "*.?fx").Union(Directory.GetFiles(folder, "*.qif")).Union(Directory.GetFiles(folder, "*.csv"));
  string doFile(string file)
  {
    Trace.WriteLine(file, "file %$#>");
    string acntFolder;
    TxMoneySrc txMoneySrc;
    foreach (var txFs in MSMoneyFileReader.ReadTxs(_dbx, file, out acntFolder, out txMoneySrc))
    {
      var txDb = _dbx.TxCoreV2s.Local.FirstOrDefault(db => string.Compare(db.FitId, txFs.FitId) == 0 || db.FitId.Contains(txFs.FitId)) ??
                 _dbx.TxCoreV2s./**/  FirstOrDefault(db => string.Compare(db.FitId, txFs.FitId) == 0 || db.FitId.Contains(txFs.FitId));
      if (txDb == null)
        _dbx.TxCoreV2s.Add(txFs);
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
            _dbx.TxCoreV2s.Add(txFs);
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
      var txDb = _dbx.BalAmtHists.FirstOrDefault(r => r.AsOfDate == txFs.AsOfDate && r.BalAmt == txFs.BalAmt && r.BalTpe == balType);
      if (txDb == null)
        _ = _dbx.BalAmtHists.Add(txFs);
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
      _txnLst.AddRange(MSMoneyFileReader.ReadTxs(_dbx, file, out var acntFolder, out var txSrcId));
    });

    await markEntriesAlreadyInDbAsync(_txnLst);

    refreshGridCalcTotals();
  }
  async Task markEntriesAlreadyInDbAsync(List<TxCoreV2> lst) => await Task.Run(() => markEntriesAlreadyInDb(lst));
  void markEntriesAlreadyInDb(List<TxCoreV2> lst)
  {
    try { foreach (var lr in lst) lr.IsInDb = _dbx.TxCoreV2s.Any(r => string.Compare(r.FitId, lr.FitId) == 0); }
    catch (NotSupportedException ex) { Trace.WriteLine("\nAP: IGNORE: " + ex.Message); }
  }
  void refreshGridCalcTotals()
  {
    ((CollectionViewSource)FindResource("txCoreV2ViewSource")).Source = _txnLst.OrderBy(r => r.IsInDb).ThenByDescending(r => r.TxAmount);
    CollectionViewSource.GetDefaultView(txCoreV2DataGrid.ItemsSource).Refresh(); //tu: refresh bound datagrid
    _balns = _txnLst.Sum(r => r.TxAmount);
    _dtMin = _txnLst.Min(r => r.TxDate);
    _dtMax = _txnLst.Max(r => r.TxDate);

    tbInfo.Text += string.Format("\r\n {1:yyyy-MM-dd} ÷ {2:yyyy-MM-dd} = {3:N1} days.    {4}/{5} new/total entries.       {0,12:c} balance.     ", _balns, _dtMin, _dtMax, (_dtMax - _dtMin).TotalDays, _txnLst.Count(r => r.IsInDb == false), _txnLst.Count());

    //btnRenameFils.Content = btnRenameFils.ToolTip = string.Format("_Rename all {0} files to DbDone", _files.Count);
  }
  void renameFilesToDbDone(List<string> files) => files.ForEach(f => renameFileToDbDone(f));
  void onClose(object s, RoutedEventArgs e) => Close();
  void renameFileToDbDone(string webFile, string? acntFolder = null)
  {
    if (webFile.Contains(PreSet.DbSynced)) { _sth.SpeakFAF($"Archive not moved."); return; }

    var newf = "";
    try
    {
      ctrlPnl.IsEnabled = false;         //too much - no way to reuse debug processed files: if (file.Contains(PreSet.DbSynced)) return;

      _bpr.Beep1of2();
      var fnwe = System.IO.Path.GetFileNameWithoutExtension(webFile);
      var tLst = MSMoneyFileReader.ReadTxs(_dbx, webFile, out acntFolder, out var txSrcId);
      if (tLst.Count() <= 0)
      {
        MessageBox.Show(acntFolder, "Something wrong.  Aborting the file.", MessageBoxButton.OK, MessageBoxImage.Warning);
        return;
      }

      var dtMin = tLst.Min(r => r.TxDate);
      var dtMax = tLst.Max(r => r.TxDate);
      var newn = string.Format("{0:yyyy-MM-dd} - {1:yyyy-MM-dd}. {3} {2:yyyy-MM-dd}", dtMin, dtMax, DateTime.Today, PreSet.DbSynced);

      newf = webFile.Replace(fnwe, newn);
      if (webFile == newf) { _sth.SpeakFAF($"\r\nNew file {Path.GetFileName(newf)} is the same as src. Aborting move."); return; }

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
      //_sth.SpeakAsyncCancelAll();
      _sth.SpeakFAF("No files are moved in debug mode.");
#else
        File.Move(webFile, newf);
#endif
      tbInfo.Text += $"\r\n - {Path.GetFileName(webFile),-20} \t=> {acntFolder,10} \\ {Path.GetFileNameWithoutExtension(newf)}   ";

      _bpr.Beep2of2();
    }
    catch (Exception ex) { ex.Pop($"renameFileToDbDone({webFile}, {acntFolder}) ===> {newf}"); }
    finally { ctrlPnl.IsEnabled = true; }
  }
}

public static class PreSet
{
  const string
    _ofc = @"C:\c\Live\MinFin\MSMoneyDbLoader\Assets\",
    _hom = @"C:\Users\alexp\OneDrive\Documents\0\MF\DnLds\";

  public const int MoneySrcPcMc = 5, MoneySrcCiVi = 8, MoneySrcTdCo = 3, MoneySrcTdPi = 2, MoneySrcUnKn = 0;

  public static string TrgFormat = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), @"0\MF\DnLds\{0}\{1}{2}");
  public const string
        AcntIdTAg = "<ACCTID>",
        __Cash = "Cash",
        __TdPi = "TdPi",
        __TdCo = "TdCo",
        __NUse = "NUse",
        __PcMc = "PcMc",
        __UnKn = "UnKn",
        __JMVi = "JMVi",
        __CiVi = "CiVi",
        __AmEx = "AmEx",
        __PcDt = "PcDt",
        __JMMc = "JMMc",
        __Amzn = "amzn",
        DbSynced = "DB-Sync-ed on",
        BaSynced = "Use later DbSynced for both";

  public static string PathToDnLdRoot => Environment.UserDomainName == "OFFICE" ? _ofc : _hom;
  public static string PathToCibcVisa => Environment.UserDomainName == "OFFICE" ? _ofc : Path.Combine(_hom, __CiVi);  // string.Format(@"{0}{1}\", hom, __CiVi); } }
  public static string PathToPcMaster => Environment.UserDomainName == "OFFICE" ? _ofc : Path.Combine(_hom, __PcMc);
  public static string PathToTdPerson => Environment.UserDomainName == "OFFICE" ? _ofc : Path.Combine(_hom, __TdPi);
  public static string PathToTdAavPro => Environment.UserDomainName == "OFFICE" ? _ofc : Path.Combine(_hom, __TdCo);
  public static string Downloads => Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders", "{374DE290-123F-4565-9164-39C4925E467B}", string.Empty).ToString(); //tu:

  public static string GetAcntFolder(string acntId)
  {
    var len = acntId.Length;
    var acnt4 = acntId.Substring(len - 4, 4);
    switch (acnt4)
    {
      case "9889": return __TdPi;
      case "9741": return __TdCo;
      case "4630": // have no idea why this is that. 
      case "9463": return __PcMc;
      case "2689": return __JMVi;
      case "2080": return __JMMc; // starting from 2016-11
      case "3729":
      case "8546": return __CiVi;
      default: break;
    }

    return acnt4;
  }
}

public class MSMoneyFileReader
{
  public static DateTime _batchTimeNow = DateTime.Now;
  static int _cntr = 0;
  private const string _separtor = "\",\"";
  private const string _header = "\"Merchant Name\",\"Card Used For Transaction\",\"Date\",\"Time\",\"Amount\"";

  public static List<TxCoreV2> ReadTxs(FinDemoContext db, string file, out string fla_acntDir, out TxMoneySrc? txMoneySrc)
  {
    Debug.WriteLine(Path.GetFileName(file), "\n\n");

    var txns = new List<TxCoreV2>();
    txMoneySrc = null;

    try
    {
      if (string.Equals(Path.GetExtension(file), ".csv", StringComparison.OrdinalIgnoreCase)) // PC since 2019
      {
        var lines = File.ReadAllLines(file);
        if (lines.Length < 2) { fla_acntDir = $"Is empty file: {file}"; return txns; }

        if (!lines[0].Equals(_header)) { fla_acntDir = $"Is not a known fin file: {file}"; return txns; }

        var acntId = lines[1].Split(new[] { _separtor }, StringSplitOptions.RemoveEmptyEntries)[1].Replace("*", "");
        txMoneySrc = getCreateTxMoneySrc(db, out fla_acntDir, acntId, Path.GetFileName(file));

        foreach (var line in lines)
        {
          if (line.Equals(_header)) continue;
          if (string.IsNullOrEmpty(line)) continue;
          var cells = line.Split(new[] { _separtor }, StringSplitOptions.RemoveEmptyEntries);
          if (cells.Length < 3) continue;

          txns.Add(new TxCoreV2
          {
            //Id = --_cntr,
            CreatedAt = _batchTimeNow,
            FitId = line,
            TxDate = parseTxnDate(cells[2], cells[3]),
            TxAmount = parseTA(cells[4].Replace("\"", "")),
            TxDetail = cells[0][1..],
            //MemoPP = mm + cs + td + si,
            TxCategoryIdTxt = PreSet.__UnKn,
            TxMoneySrc = txMoneySrc,
            TxMoneySrcId = txMoneySrc.Id,
            SrcFile = Path.GetFileNameWithoutExtension(file),
            Notes = $"{file}" // 2021-01
          });
        }
      }
      else if (string.Equals(Path.GetExtension(file), ".qif", StringComparison.OrdinalIgnoreCase)) // amazon / chase qif file format
      {
        var blocks = File.ReadAllText(file).Split(new string[] { "^" }, StringSplitOptions.RemoveEmptyEntries);
        var acntId = "amzn"; //only for amazon since no other formats available; note: no CC # in qif ==> use for amazon only.
        txMoneySrc = getCreateTxMoneySrc(db, out fla_acntDir, acntId, Path.GetFileName(file));

        foreach (var block in blocks)
          if (block.Contains("\nD"))
            txns.Add(doTxnBlockQif(block, file, txMoneySrc));
      }
      else // .ofx .qfx .aso
      {
        var blocks = File.ReadAllText(file).Split(new string[] { "</STMTTRN>" }, StringSplitOptions.RemoveEmptyEntries);

        var acntId = blocks[0].Split(new string[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault(r => r.Contains(PreSet.AcntIdTAg));
        if (acntId?.Length > 20) // Jun2017: elements not always are one per line.
          acntId = getElCont(acntId, PreSet.AcntIdTAg);

        txMoneySrc = getCreateTxMoneySrc(db, out fla_acntDir, acntId, Path.GetFileName(file));

        foreach (var block in blocks)
          if (block.Contains("<STMTTRN>"))
            txns.Add(doTxnBlock(block.Replace("\n", "").Replace("\r", ""), file, txMoneySrc));
      }
    }
    catch (Exception ex) { _ = ex.Log(); fla_acntDir = "Exceptioned"; }

    return txns;
  }

  static TxMoneySrc getCreateTxMoneySrc(FinDemoContext db, out string acntDir, string acntId, string filename)
  {
    var ad = acntDir = PreSet.GetAcntFolder((acntId ?? "AcntUnknown").Replace(PreSet.AcntIdTAg, "").Trim());
    var txMoneySrcs = db.TxMoneySrcs.FirstOrDefault(r => r.Fla.ToLower() == ad.ToLower());
    if (txMoneySrcs == null)
    {
      // Fix: Access the `Entity` property of the returned `EntityEntry<T>` to get the actual entity.
      txMoneySrcs = db.TxMoneySrcs.Add(new TxMoneySrc
      {
        CreatedAt = _batchTimeNow,
        Fla = acntDir,
        Name = acntDir,
        Notes = $"Auto created from '{filename}'"
      }).Entity;

      //db.SaveChanges();
      var rs = DbSaveMsgBox.CheckAskSave(db);

      new System.Speech.Synthesis.SpeechSynthesizer().Speak($"New account {acntDir} created from the file {filename}.");
    }

    if (txMoneySrcs == null)
    {
      if (Debugger.IsAttached)
        Debugger.Break();
      else
        throw new NullReferenceException($"Unable to establish source account. Exiting");
    }

    return txMoneySrcs;
  }

  public const string LedgerBal = "LEDGERBAL", AvailbBal = "AVAILBAL";
  public static List<BalAmtHist> ReadBAH(string file, TxMoneySrc txMoneySrc, string balTpe = LedgerBal) // for all files, or AVAILBAL for VIsa, TD only.
  {
    var rv = new List<BalAmtHist>();

    var tagBgn = string.Format("<{0}>", balTpe);
    var tagEnd = string.Format("</{0}>", balTpe);
    var blocks = File.ReadAllText(file).Split(new string[] { tagEnd }, StringSplitOptions.RemoveEmptyEntries);
    foreach (var block in blocks)
      if (block.Contains(tagBgn))
        rv.Add(doBalAmtBlock(block.Replace("\n", "").Replace("\r", ""), txMoneySrc, tagBgn, balTpe));

    return rv;
  }

  static int inferTxMoneySrcId(string file)
  {
    var dirs = file.Split('\\');
    var dir = dirs[^2];
    return dir switch
    {
      PreSet.__Cash => 1,//1	 Cash	
      PreSet.__TdPi => 2,//2	 Debit*1301 (Alex)	
      PreSet.__TdCo => 3,//3	 biz Debit*3368 (AAVpro)	Debit Card
      PreSet.__NUse => 4,//4	 zVISA (old CIBC)	NOT USED
      PreSet.__PcMc => 5,//5	 MC*9463 (Alex)	
      PreSet.__UnKn => 6,//6	 New Way!	not sure ..cheques...
      PreSet.__JMVi => 7,//7	 wVISA*2689 (Mei)	
      PreSet.__CiVi => 8,//8	 VISA*3729 (Alex)	
      PreSet.__AmEx => 9,//9	 AmEx (Mei)	
      PreSet.__PcDt => 10,//a	 PC Debit*???? (Mei)	Meis' chequing accnt}
      PreSet.__Amzn => 13,
      PreSet.__JMMc => 13,
      _ => 6,//0	 is not working with db ini.
    };
  }
  static BalAmtHist doBalAmtBlock(string block, TxMoneySrc txMoneySrc, string tagBgn, string balTpe)
  {
    var start = block.IndexOf(tagBgn);
    if (start < 0) return null;

    var dp = getElCont(block, "<DTASOF>"); // as of date
    var ta = getElCont(block, "<BALAMT>");

    var rv = new BalAmtHist
    {
      //Id = --_cntr,
      CreatedAt = _batchTimeNow,
      AsOfDate = parseTxnDate(dp),
      BalAmt = -parseTA(ta),
      BalTpe = balTpe,
      TxMoneySrc = txMoneySrc,
      TxMoneySrcId = txMoneySrc.Id // inferTxMoneySrcId(file), // fix this Jan 30 2019
    };

    return rv;
  }
  static TxCoreV2 doTxnBlockQif(string block, string file, TxMoneySrc txMoneySrc)
  {
    try
    {
      var ls = block.Split(new[] { "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries);

      var rv = new TxCoreV2
      {
        //Id = --_cntr,
        CreatedAt =          /**/  _batchTimeNow,
        FitId =              /**/  ls.FirstOrDefault(r => r[0] == 'N')[1..],
        TxDate =             /**/  parseTxnDate(ls.FirstOrDefault(r => r[0] == 'D')[1..]),
        TxAmount =           /**/  -parseTA(ls.FirstOrDefault(r => r[0] == 'T')[1..]),
        TxDetail =           /**/  ls.FirstOrDefault(r => r[0] == 'P')[1..],
        MemoPp =             /**/  string.Join(", ", ls.Where(r => r[0] == 'A' && r.Length > 1).Select(r => r[1..])) + ". ",
        TxCategoryIdTxt =    /**/  PreSet.__UnKn,
        TxMoneySrc =         /**/  txMoneySrc,
        TxMoneySrcId =       /**/  txMoneySrc.Id, // inferTxMoneySrcId(file),
        SrcFile =            /**/  Path.GetFileNameWithoutExtension(file)
      };

      Debug.WriteLine(rv);

      return rv;
    }
    catch (Exception ex) { _ = ex.Log(); }

    return null;
  }
  static TxCoreV2 doTxnBlock(string block, string file, TxMoneySrc txMoneySrc)
  {
    var start = block.IndexOf("<STMTTRN>");
    if (start < 0) return null;

    //Debug.WriteLine(block.Substring(33).Replace("<TRNAMT>", "\t").Replace("<FITID>", "\t").Replace("<NAME>", "\t\t").Replace("<MEMO>", "\t\t").Replace("<CHECKNUM>", "\t\t").Replace("<DTUSER>", "\t\t").Replace("<SIC>", "\t\t"));

    //var tt = getElCont(block, "<TRNTYPE>");
    var dp = getElCont(block, "<DTPOSTED>");     // posted date
    var ta = getElCont(block, "<TRNAMT>");
    var fi = getElCont(block, "<FITID>");
    var dt = getElCont(block, "<NAME>") ?? "°";  // required db field
    var mm = getElCont(block, "<MEMO>");         // CV only: details of $US txn.
    var cs = getElCont(block, "<CHECKNUM>");     // TD only: cheque number
    var td = getElCont(block, "<DTUSER>");       // MC only: tx date
    var si = getElCont(block, "<SIC>");          // MC only: 4-digits like 0000, 4816, ...

    if (td != null) td = string.Format("txDate:{0}, ", parseTxnDate(td));
    if (si != null) si = string.Format("sic:{0}", si);

    var rv = new TxCoreV2
    {
      //Id = --_cntr,
      CreatedAt = _batchTimeNow,
      FitId = fi,
      TxDate = parseTxnDate(dp),
      TxAmount = -parseTA(ta),
      TxDetail = dt,
      MemoPp = mm + cs + td + si,
      TxCategoryIdTxt = PreSet.__UnKn,
      TxMoneySrc = txMoneySrc,
      TxMoneySrcId = txMoneySrc.Id, // inferTxMoneySrcId(file),
      SrcFile = Path.GetFileNameWithoutExtension(file)
    };

    //Debug.WriteLine(rv);

    return rv;
  }
  static DateTime parseTxnDate(string mdy, string hmm_m)
  {
    if (!DateTime.TryParseExact($"{mdy} {hmm_m}", "MM/dd/yyyy h:mm tt", CultureInfo.InvariantCulture, DateTimeStyles.None, out var ta))
      throw new Exception($"Bad date:    {mdy} {hmm_m}");

    if (ta > _batchTimeNow)
    {
      Trace.WriteLine($"PC MC bug: date corrected from future {ta} to past {ta.AddYears(-1)}");
      ta = ta.AddYears(-1);
    }

    return ta;
  }
  static DateTime parseTxnDate(string dp)
  {
    var dv = dp.Trim().Split('[')[0];
    var f = "yyyyMMddHHmmss.fff"[..dv.Length];
    if (DateTime.TryParseExact(dv, f, CultureInfo.InvariantCulture, DateTimeStyles.None, out var ta))
      return ta;

    f = "MM/dd/yyyy";
    if (DateTime.TryParseExact(dv, f, CultureInfo.InvariantCulture, DateTimeStyles.None, out ta))
      return ta;

    {
      Debug.WriteLine("\n{0}\n{1}", f, dp);
      throw new Exception("Bad amount: " + dp);
    }
  }
  static decimal parseTA(string ts) => !decimal.TryParse(ts.Replace("$", ""), out var ta) ? throw new Exception("Bad amount: " + ts) : ta;
  static string getElCont(string block, string p)
  {
    var start = block.IndexOf(p);
    if (start < 0) return null;

    var rv = block[(start + p.Length)..];
    var r2 = rv.Split('<')[0];      //Debug.WriteLine("    '{0}' {1}", r2, "    ");
    return r2.Trim();
  }
}

/*
 Using the new code (LinqToXml)

 XElement doc = ImportOfx.ToXElement(pathToOfx);
//queryiny the XElement
var imps = (from c in doc.Descendants("STMTTRN")
            where c.Element("TRNTYPE").Value == "DEBIT"
            select new tb_import
            {
                amount = decimal.Parse(c.Element("TRNAMT").Value.Replace("-", ""),
                                       NumberFormatInfo.InvariantInfo),
                data = DateTime.ParseExact(c.Element("DTPOSTED").Value, 
                                           "yyyyMMdd", null),
                description = c.Element("MEMO").Value,
                id_account = id_account
            });

 * These are the columns inside the DataSet:

TRNTYPE - Type of transaction: DEBIT or CREDIT
DTPOSTED - Date of the transaction, formatted YYYYMMDDHHMMSS
TRNAMT - Amount (negative when it is a DEBIT)
FITID - Transaction ID CHECKNUM - Number of the check or transaction ID
MEMO - Small description of the transaction; very useful when you use your debit card

 * 
 
 The plan
 * new db schema:
 *	- one TxCore for all possible types:
 *		contains FitId string column
 *		see if we miss anything from CSV (txn vs post date: have to use post since not in MsMoney.)
 * import all into the new paralell schema and see if totals match
 *		
 * 
 */
