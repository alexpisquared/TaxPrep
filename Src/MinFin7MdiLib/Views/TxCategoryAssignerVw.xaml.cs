﻿namespace MF.TxCategoryAssigner;
public partial class TxCategoryAssignerVw : WindowBase
{
  readonly FinDemoContext _dbx;
  readonly DateTime _yrStart2004 = new(2004, 1, 1), _now = DateTime.Now;
  readonly ObservableCollection<TxCoreV2> _txCoreItems = [];
  readonly ObservableCollection<TxCategory> _txCategories = [];
  readonly CollectionViewSource _txCategoryCmBxVwSrc, _txCategoryDGrdVwSrc, _txCoreV2_Root_VwSrc;
  readonly int _trgTaxYr = DateTime.Today.Year - 1;
  readonly ILogger _lgr;
  readonly IBpr _bpr;
  private readonly SpeechSynth _sth;
  string? _txCatgry, _loadedCatgry = "?", _choiceAbove, _choiceBelow;
  decimal _selectTtl = 0;
  bool _loaded = false;
  int? _cutOffYr = null;

  public TxCategoryAssignerVw(ILogger lgr, IBpr bpr, SpeechSynth sth, FinDemoContext dbx)
  {
    InitializeComponent();
    _txCategoryCmBxVwSrc = (CollectionViewSource)FindResource("txCategoryCmBxVwSrc");
    _txCategoryDGrdVwSrc = (CollectionViewSource)FindResource("txCategoryDGrdVwSrc");
    _txCoreV2_Root_VwSrc = (CollectionViewSource)FindResource("txCoreV2_Root_VwSrc");
    _lgr = lgr;
    _bpr = bpr;
    _sth = sth;
    _dbx = dbx;
  }
  async void OnLoaded(object s, RoutedEventArgs e)
  {
    /*
     * var version = Windows.ApplicationModel.Package.Current.Id.Version;
     * 
You need to install the Windows 10 WinRT API pack. Install from NuGet the package: Microsoft.Windows.SDK.Contracts

URL: https://www.nuget.org/packages/Microsoft.Windows.SDK.Contracts

Then you can do something like this:

var version = Windows.ApplicationModel.Package.Current.Id.Version;
applicationVersion = string.Format("{0}.{1}.{2}.{3}",
    version.Major,
    version.Minor,
    version.Build,
    version.Revision);
If you want to DEBUG or Run with the current Package available, just set your package deployment project as the Startup Project.     */

    try
    {
      await _dbx.TxCategories.LoadAsync();
      await _dbx.TxMoneySrcs.LoadAsync();

      _txCategoryDGrdVwSrc.Source = _dbx.TxCategories.Local.OrderBy(r => r.ExpGroup?.Name).ThenBy(r => r.TlNumber);
      _txCategoryCmBxVwSrc.Source = _dbx.TxCategories.Local.OrderBy(r => r.Name).ThenBy(r => r.TlNumber);

      _ = tbxSearch.Focus();
      var y = DateTime.Today.Year;
      cbxSingleYr.ItemsSource = new int[] { y, y - 1, y - 2, y - 3, y - 4, y - 5, y - 6, y - 7, y - 8, y - 9 };
      cbxSingleYr.SelectedIndex = 2;
      chkInfer.IsChecked = true;

      btAssign.IsEnabled = false;

      _loaded = true;
      chkSingleYr.IsChecked = true;      // invokes the 1st search
    }
    catch (Exception ex) { ex.Pop(_lgr); }
  }
  async void OnSave(object s, RoutedEventArgs e) => await SaveAsync(); async Task SaveAsync()
  {
    try
    {
      var (success, _, report) = await _dbx.TrySaveReportAsync();
      Title = tbxNew.Text = report;

      if (!success)
        _ = MessageBox.Show(report, $"Attach to process to debug/fix!!! ");
    }
    catch (Exception ex) { ex.Pop(_lgr); }
  }
  protected override async void OnClosing(CancelEventArgs e)
  {
    if (_dbx.HasUnsavedChanges())
    {
      var rsn = $"Would you like to save the changes? \r\n\n{_dbx.GetDbChangesReport()}";
      switch (MessageBox.Show(rsn, "Unsaved changes detected", MessageBoxButton.YesNoCancel, MessageBoxImage.Question))
      {
        default:
        case MessageBoxResult.Cancel: e.Cancel = true; KeepOpenReason = "Changes have not been saved... \n...Cancelling the Closing in the Parent of base."; return;
        case MessageBoxResult.Yes: await SaveAsync(); KeepOpenReason = null; await Task.Delay(250); break;
        case MessageBoxResult.No: break;
      }
    }
    else
    {
      KeepOpenReason = "";
    }

    base.OnClosing(e);
  }

  async Task ReLoadTxCore() // limit txns by chkboxs' filters
  {
    if (!_loaded) return;

    var started = Stopwatch.GetTimestamp();

    try
    {
      if (chkTxCatgry.IsChecked == true && chkSingleYr.IsChecked == true)
      {
        await _dbx.TxCoreV2s.Where(r => r.TxDate.Year >= _cutOffYr && r.TxCategoryIdTxt == _txCatgry).LoadAsync();
        _loadedCatgry = _txCatgry;
      }
      else if (chkTxCatgry.IsChecked == true)
      {
        await _dbx.TxCoreV2s.Where(r => r.TxDate >= _yrStart2004 && r.TxCategoryIdTxt == _txCatgry).LoadAsync();
        _loadedCatgry = _txCatgry;
      }
      else if (chkSingleYr.IsChecked == true)
      {
        await _dbx.TxCoreV2s.Where(r => r.TxDate.Year >= _cutOffYr).LoadAsync();
      }
      else
      {
        await _dbx.TxCoreV2s.Where(r => r.TxDate >= _yrStart2004).LoadAsync();
        _loadedCatgry = _txCatgry = null;
      }

      if (dgTxCore.ItemsSource != null)
      {
        CollectionViewSource.GetDefaultView(dgTxCore.ItemsSource).Refresh(); //tu:            
      }

      UpdateTitle(Stopwatch.GetElapsedTime(started));
    }
    catch (Exception ex) { ex.Pop(_lgr); }
  }
  async Task FilterStart(string csvFilterString)
  {
    if (!_loaded) return;

    var started = Stopwatch.GetTimestamp();

    try
    {
      WriteLine($"■■■ filterStart()  [[");

      _bpr.Tick();

      if (string.IsNullOrEmpty(csvFilterString))
      {
        FilterTxnsBy2(csvFilterString, _txCatgry);

        var lastSelectedItem = dgTxCore.SelectedItems.Count > 0 ? dgTxCore.SelectedItems[dgTxCore.SelectedItems.Count - 1] : null;
        if (lastSelectedItem is not null)
        {
          dgTxCore.SelectedItem = lastSelectedItem;
          dgTxCore.ScrollIntoView(lastSelectedItem);
        }

        //_sth.SpeakFAF("Yes");
      }
      else
      {
        var ta = csvFilterString.Split(['`', '>', '\\', '/']);
        if (ta.Length > 1)
        {
          if (string.IsNullOrEmpty(ta[0]) && string.IsNullOrEmpty(ta[1])) { _sth.SpeakFAF("Both are empty."); return; }

          if (string.IsNullOrEmpty(ta[0]))
          {
            FilterTxnsBy2(ta[1], _txCatgry);
          }
          else
          {
            if (!decimal.TryParse(ta[0], out var amt)) { _sth.SpeakFAF("1st must be number."); return; }

            if (!decimal.TryParse(tbxRange.Text, out var rng)) tbxRange.Text = (rng = 0m).ToString();

            //_sth.SpeakFAF("Multi.");
            FilterTxnsBy4(csvFilterString[(ta[0].Length + 1)..], _txCatgry, amt, rng);
          }
        }
        else // == 1
        {
          if (ta.Length != 1) _sth.SpeakFAF($"{ta.Length}-part filter in action.");

          if (decimal.TryParse(csvFilterString, out var amt))
          {
            if (!decimal.TryParse(tbxRange.Text, out var rng)) tbxRange.Text = (rng = 0m).ToString();

            FilterTxnsBy4("", _txCatgry, amt, rng);
          }
          else
          {
            FilterTxnsBy2(csvFilterString, _txCatgry);
          }
        }
      }

      _txCoreV2_Root_VwSrc.Source = _txCoreItems;

      if (chkInfer.IsChecked == true)
      {
        var catIds = _txCoreItems.Select(r => r.TxCategoryIdTxtNavigation.Id).Distinct().Where(r => r != 3); // Debug.WriteLine($"catIds.Count() = {catIds.Count()}");
        FilterCategoryByIdList(catIds);
      }

      UpdateTitle(Stopwatch.GetElapsedTime(started));
    }
    catch (Exception ex) { ex.Pop(_lgr); }
    finally { WriteLine($"■■■ filterStart()  ]]"); _bpr.Tick(); await Task.Yield(); }
  }
  void FilterTxnsBy2(string csvFilterString, string? txCategoryId, [CallerMemberName] string? cmn = "")
  {
    var started = Stopwatch.GetTimestamp();
    tbkFlt.Text = csvFilterString;
    tbkAmt.Text = "";

    _txCoreItems.ClearAddRangeAuto(_dbx.TxCoreV2s.Local.Where(r =>
      (
        (!string.IsNullOrEmpty(r.TxDetail) && r.TxDetail.ToLower().Contains(csvFilterString.ToLower())) ||
        (!string.IsNullOrEmpty(r.MemoPp) && r.MemoPp.ToLower().Contains(csvFilterString.ToLower())) ||
        (!string.IsNullOrEmpty(r.Notes) && r.Notes.ToLower().Contains(csvFilterString.ToLower()))
      )
      &&
      (
        _cutOffYr == null ? r.TxDate >= _yrStart2004 : r.TxDate.Year >= _cutOffYr
      )
      &&
      (
        string.IsNullOrEmpty(txCategoryId) || string.Compare(r.TxCategoryIdTxt, txCategoryId, true) == 0
      )
    ).OrderByDescending(r => r.TxDate));

    WriteLine($" === {Stopwatch.GetElapsedTime(started).TotalSeconds,4:N1}s   {cmn}");
  }
  void FilterTxnsBy4(string csvFilterString, string? txCategoryId, decimal txnAmt, decimal range, [CallerMemberName] string? cmn = "")
  {
    var started = Stopwatch.GetTimestamp();
    tbkFlt.Text = csvFilterString;
    tbkAmt.Text = $"{txnAmt:N0}";

    _txCoreItems.ClearAddRangeAuto(_dbx.TxCoreV2s.Local.Where(r =>
      txnAmt - range <= (chkIsAbs.IsChecked == true ? Math.Abs(r.TxAmount) : r.TxAmount) && (chkIsAbs.IsChecked == true ? Math.Abs(r.TxAmount) : r.TxAmount) <= txnAmt + range
      &&
      (
        (!string.IsNullOrEmpty(r.TxDetail) && r.TxDetail.ToLower().Contains(csvFilterString.ToLower())) ||
        (!string.IsNullOrEmpty(r.MemoPp) && r.MemoPp.ToLower().Contains(csvFilterString.ToLower())) ||
        (!string.IsNullOrEmpty(r.Notes) && r.Notes.ToLower().Contains(csvFilterString.ToLower()))
      )
      &&
      (
        _cutOffYr == null ? r.TxDate >= _yrStart2004 : r.TxDate.Year >= _cutOffYr
      )
      &&
      (
        string.IsNullOrEmpty(txCategoryId) || string.Compare(r.TxCategoryIdTxt, txCategoryId, true) == 0
      )
    ).OrderByDescending(r => r.TxDate));

    WriteLine($" === {Stopwatch.GetElapsedTime(started).TotalSeconds,4:N1}s   {cmn}");
  }
  void FilterCategoryByIdList(IEnumerable<int> catIds) => UpdateCtgrList(_dbx.TxCategories.Local.Where(r => catIds.Contains(r.Id)));
  void FilterCategoryByTxtMatch(string namePart/**/  ) => UpdateCtgrList(_dbx.TxCategories.Local.Where(r => r.Name.ToLower().Contains(namePart) || r.IdTxt.ToLower().Contains(namePart)));

  void UpdateCtgrList(IEnumerable<TxCategory> enu)
  {
    var started = Stopwatch.GetTimestamp();

    _txCategories.ClearAddRangeAuto(enu.OrderBy(r => r.ExpGroup?.Name).ThenBy(r => r.TlNumber));

    ArgumentNullException.ThrowIfNull(_txCategoryDGrdVwSrc);
    _txCategoryDGrdVwSrc.Source = _txCategories;

    if (_txCategories.Count == 0)
    {
      _txCategories.ClearAddRangeAuto(_dbx.TxCategories.Local.OrderBy(r => r.ExpGroup?.Name).ThenBy(r => r.TlNumber));
    }

    choiceAbove.IsEnabled = choiceBelow.IsEnabled = btAssign.IsEnabled = false;
    _choiceAbove = _choiceBelow = "";

    if (_txCategories.Count == 1)
    {
      btAssign.IsEnabled = true;
      _ = _txCategoryDGrdVwSrc.View.MoveCurrentToFirst();
    }
    else if (_txCategories.Count == 2)
    {
      _choiceAbove = _txCategories.First().IdTxt;
      _choiceBelow = _txCategories.Last().IdTxt;
      btAssign.IsEnabled = txCategoryListBox.SelectedItems.Count == 1;// _choiceAbove == _choiceBelow;
    }

    choiceAbove.Content = $"_1 {_choiceAbove}"; choiceAbove.IsEnabled = _choiceAbove.Length > 0;
    choiceBelow.Content = $"_7 {_choiceBelow}"; choiceBelow.IsEnabled = _choiceBelow.Length > 0;

    UpdateTitle(Stopwatch.GetElapsedTime(started));
  }

  static async Task<bool> IsStillTyping(TextBox textbox) { var prev = textbox.Text; await Task.Delay(750); return prev != textbox.Text; }
  void UpdateTitle(TimeSpan took, [CallerMemberName] string? cmn = "")
  {
    WriteLine(Title = $"Ctgry {_loadedCatgry,-12} {_txCoreItems.Count,9:N0} rows    sum {_txCoreItems.Where(r => r.TxCategoryIdTxt == "UnKn").Sum(r => r.TxAmount),9:N0} / {_txCoreItems.Sum(r => r.TxAmount),-9:N0} \t selects {_selectTtl,9:N2} \t {took.TotalSeconds,4:N2}s \t {cmn} \t\t {VersionHelper.CurVerStr}");
    h0.Text = $"{_txCoreItems.Count,9:N0} rows     ";
    h1.Text = $"$UnKn/All {_txCoreItems.Where(r => r.TxCategoryIdTxt == "UnKn").Sum(r => r.TxAmount):N0} / {_txCoreItems.Sum(r => r.TxAmount):N0}     ";
    h2.Text = $"Selects {_selectTtl:N2}     ";
    h3.Text = $"{VersionHelper.CurVerStr}     ";
    h4.Text = $"Db: {_dbx.Database.GetDbConnection().Database}     ";
  }

  async void OnReLoad(object s, RoutedEventArgs e)
  {
    switch (DbSaveMsgBox.CheckAskSave(_dbx))
    {
      case (int)MsgBoxDbRslt.Yes: break;
      default: break;
      case (int)MsgBoxDbRslt.Cancel: return;
      case (int)MsgBoxDbRslt.No: break;
    }

    await ReLoadTxCore();
    OnClear1(s, e);
  }

  async void OnTextChangedFuz(object s, TextChangedEventArgs e) { if (!await IsStillTyping((TextBox)s)) await FilterStart(tbxSearch.Text); }
  async void OnTextChangedCtg(object s, TextChangedEventArgs e) { if (!await IsStillTyping((TextBox)s)) FilterCategoryByTxtMatch(((TextBox)s).Text); }

  void OnClear1(object s, RoutedEventArgs e) => tbkFlt.Text = tbxSearch.Text = "";
  void OnClear2(object s, RoutedEventArgs e) => tSrch2.Text = "";

  void DgTxCtgr_SelChd(object s, SelectionChangedEventArgs e)
  {
    btAssign.IsEnabled = e.AddedItems.Count == 1;
    btAssign.Content = e.AddedItems.Count == 1 ? $"{((TxCategory?)e.AddedItems[0])?.IdTxt}" : "? ? ?";
  }

  void DgTxCore_SelChd(object s, SelectionChangedEventArgs e)
  {
    _selectTtl = 0;
    if (e.AddedItems.Count < 1)
    {
      btnNotePaster.Content = "";
      btnNotePaster.IsEnabled = false;
      return;
    }

    try
    {
      var started = Stopwatch.GetTimestamp();

      _choiceAbove = _choiceBelow = "";

      btnNotePaster.IsEnabled = dgTxCore.SelectedItems.Count > 0 && Clipboard.ContainsText() && Clipboard.GetText().Length > 0;
      btnNotePaster.Content = btnNotePaster.IsEnabled ? $"'{Clipboard.GetText()}'\n_Paste to {dgTxCore.SelectedItems.Count} rows  ▼" : "¦";

      if (dgTxCore.SelectedItems.Count > 0)
      {
        var lv = new List<TxCoreV2>();
        foreach (TxCoreV2 tc in dgTxCore.SelectedItems)
        {
          lv.Add(tc);
          _selectTtl += tc.TxAmount;
        }

        if (dgTxCore.SelectedItems.Count == 1)
        {
          var select = dgTxCore.SelectedItems[0] as TxCoreV2;
          ArgumentNullException.ThrowIfNull(select, "#18 ▄▀▄▀▄▀▄▀▄▀▄▀");

          var dlrAmntMatches = _dbx.TxCoreV2s.Local.Where(r => (_cutOffYr == null ? r.TxDate >= _yrStart2004 : r.TxDate.Year >= _cutOffYr) && Math.Abs(r.TxAmount) == Math.Abs(select.TxAmount)).OrderByDescending(r => r.TxDate);
          var detailsMatches = _dbx.TxCoreV2s.Local.Where(r => (_cutOffYr == null ? r.TxDate >= _yrStart2004 : r.TxDate.Year >= _cutOffYr) && !string.IsNullOrEmpty(r.TxDetail) && r.TxDetail.ToLower().Contains(select.TxDetail.ToLower())).OrderByDescending(r => r.TxDate);
          var memoStrMatches = _dbx.TxCoreV2s.Local.Where(r => (_cutOffYr == null ? r.TxDate >= _yrStart2004 : r.TxDate.Year >= _cutOffYr) && !string.IsNullOrEmpty(r.MemoPp) && !string.IsNullOrEmpty(select.MemoPp) && r.MemoPp.ToLower().Contains(select.MemoPp.ToLower())).OrderByDescending(r => r.TxDate);

          //tbxNew.Text = $"{(dlrAmntMatches?.Count() ?? 1) - 1,-5}   ttl$ {dlrAmntMatches?.Sum(r => r.TxAmount) ?? 0m,-8:N0}       TxDtl {(detailsMatches?.Count() ?? 1) - 1,-8}    Mem {(memoStrMatches?.Count() ?? 1) - 1,-8}";

          btnMatchesByAmount.Content = $"+ {(dlrAmntMatches?.Count() ?? 1) - 1}";
          btnMatchesByTxDetl.Content = $"TxDetail  +{(detailsMatches?.Count() ?? 1) - 1}";
          btnMatchesByMemoPP.Content = $"MemoPP  +{(memoStrMatches?.Count() ?? 1) - 1}";
        }
        else
        {
          var trg = lv.Where(r => r.TxCategoryIdTxtNavigation.IdTxt != "UnKn")?.FirstOrDefault();
          _choiceAbove = _choiceBelow = trg?.TxCategoryIdTxtNavigation.IdTxt ?? "";
          btAssign.IsEnabled = txCategoryListBox.SelectedItems.Count == 1;
        }
      }

      choiceAbove.Content = $"_1 {_choiceAbove}"; choiceAbove.IsEnabled = _choiceAbove.Length > 0;
      choiceBelow.Content = $"_7 {_choiceBelow}"; choiceBelow.IsEnabled = _choiceBelow.Length > 0;

      btAssign.IsEnabled = _choiceAbove.Length > 0 && _choiceAbove == _choiceBelow; //todo: assign does not work!!! 2023-01-15

      UpdateTitle(Stopwatch.GetElapsedTime(started));
    }
    catch (Exception ex) { ex.Pop(_lgr); }
  }

  async void OnTxCatgry_Checked(object s, RoutedEventArgs e)      /**/ { _ = cbxTxCatgry.Focus(); await TccTsk(); }
  async void OnTxCatgry_UnChckd(object s, RoutedEventArgs e)      /**/ { if (!_loaded) return; /**/                                   _txCatgry = null;                         /**/ await ReLoadTxCore(); await FilterStart(tbxSearch.Text); }
  async void OnTxCatgry_Changed(object s, SelectionChangedEventArgs e) => await TccTsk(); async Task TccTsk() { if (!_loaded) return; _txCatgry = (string)cbxTxCatgry.SelectedValue; await ReLoadTxCore(); await FilterStart(tbxSearch.Text); }

  async void OnSingleYr_Checked(object s, RoutedEventArgs e)      /**/ { _ = cbxSingleYr?.Focus(); await SycTsk(); }
  async void OnSingleYr_UnChckd(object s, RoutedEventArgs e)      /**/ { if (!_loaded) return; /**/                                   _cutOffYr = null;                      /**/ await ReLoadTxCore(); await FilterStart(tbxSearch.Text); }
  async void OnSingleYr_Changed(object s, SelectionChangedEventArgs e) => await SycTsk(); async Task SycTsk() { if (!_loaded) return; _cutOffYr = (int)cbxSingleYr.SelectedValue; await ReLoadTxCore(); await FilterStart(tbxSearch.Text); }

  void OnLbxSelChd(object s, SelectionChangedEventArgs e)
  {
    if (e.AddedItems.Count < 1) return;

    var started = Stopwatch.GetTimestamp();
    UpdateTitle(Stopwatch.GetElapsedTime(started));

    Title += $" - {((TxCategory?)txCategoryListBox.SelectedItems[0])?.Name}";
  }
  async void OnInfer_Checked(object s, RoutedEventArgs e) => await FilterStart(tbkFlt.Text);

  void OnFindByDlr(object sender, RoutedEventArgs e)
  {
    tbxRange.Text = "0.00"; // :only the exact matches are OK here.
    tbxSearch.Text = $"{Math.Abs((dgTxCore.SelectedItem as TxCoreV2)?.TxAmount ?? 0m):N2}";
    Clipboard.SetText(tbxSearch.Text);
  }
  void OnFindByDtl(object sender, RoutedEventArgs e) => Clipboard.SetText(tbxSearch.Text = (dgTxCore.SelectedItem as TxCoreV2)?.TxDetail ?? "");
  void OnFindByMem(object sender, RoutedEventArgs e) => Clipboard.SetText(tbxSearch.Text = (dgTxCore.SelectedItem as TxCoreV2)?.MemoPp ?? "");

  void DgTxCtgr_MouseDblClick(object s, MouseButtonEventArgs e) { if (btAssign.IsEnabled) OnAssign0(s, e); }
  void DgTxCore_MouseDblClick(object s, MouseButtonEventArgs e)
  {
    if (e.OriginalSource is System.Windows.Documents.Run run)
      Clipboard.SetText(tbxSearch.Text = run.Text);
    else if (((TxCoreV2)((System.Windows.Controls.Primitives.Selector)s).SelectedItem) is not null)
      Clipboard.SetText(tbxSearch.Text = ((TxCoreV2)((System.Windows.Controls.Primitives.Selector)s).SelectedItem)?.TxDetail);
    else
      return;

    _ = tbxSearch.Focus(); // _sth.SpeakFAF("Also, Clipboarded.");
  }

  //use main menu instead: async void OnManualTxnAdd(object s, RoutedEventArgs e) { _ = new ManualTxnEntry(_lgr, _bpr, false, _dba).ShowDialog(); await ReLoadTxCore(); }
  async void OnAssign0(object s, RoutedEventArgs e)
  {
    if (txCategoryListBox.SelectedItems.Count == 1)
      await Assign(((TxCategory?)txCategoryListBox.SelectedItems[0])?.IdTxt);
    else if (_choiceAbove?.Length > 0 && _choiceAbove == _choiceBelow)
      await Assign(_choiceAbove);
    //else      _sth.SpeakFAF("Nothing to assign.");
  }
  async void OnAssign1(object s, RoutedEventArgs e) => await Assign(_choiceAbove);
  async void OnAssign2(object s, RoutedEventArgs e) => await Assign(_choiceBelow);
  async void OnPasteToNote(object s, RoutedEventArgs e) => await PasteToNote(Clipboard.ContainsText() ? Clipboard.GetText() : "");
  async Task PasteToNote(string? newNoteText)
  {
    if (dgTxCore.SelectedItems.Count < 1 || newNoteText is null)
    {
      btnNotePaster.Content = "";
      btnNotePaster.IsEnabled = false;
      return;
    }

    foreach (TxCoreV2 tc in dgTxCore.SelectedItems) { tc.Notes = newNoteText; tc.ModifiedAt = _now; tc.ModifiedBy = Environment.UserName; }

    await SaveChangesAndClearIfAsync(false);
  }
  async Task Assign(string? IdTxt)
  {
    ArgumentNullException.ThrowIfNull(IdTxt, "▄▀▄▀▄▀▄▀▄▀▄▀");

    btAssign.IsEnabled = choiceAbove.IsEnabled = choiceBelow.IsEnabled = false;

    if (dgTxCore.SelectedItems.Count > 0)
    {
      foreach (TxCoreV2 tc in dgTxCore.SelectedItems) { tc.TxCategoryIdTxt = IdTxt; tc.ModifiedAt = _now; tc.ModifiedBy = Environment.UserName; }
    }
    else if (tbxSearch.Text.Length > 1)
    {
      foreach (var tc in _txCoreItems)
      {
        if (tc.TxCategoryIdTxt.Equals("UnKn", StringComparison.OrdinalIgnoreCase)) // if (!tc.TxCategoryIdTxt.Equals(IdTxt, StringComparison.OrdinalIgnoreCase))
        {
          tc.TxCategoryIdTxt = IdTxt; tc.ModifiedAt = _now; tc.ModifiedBy = Environment.UserName;
        }
      }
    }

    await SaveChangesAndClearIfAsync(true);

    _ = tbxSearch.Focus();
  }

  async Task SaveChangesAndClearIfAsync(bool clear)
  {
    var rows = DbSaveMsgBox.TrySaveAsk(_dbx, $"class TxCategoryAssignerVw.assign()");
    tbxNew.Text = $"{rows} saved.";
    if (rows > 0)
    {
      await ReLoadTxCore();

      if (clear)
      {
        OnClear1(default!, default!);
        OnClear2(default!, default!);
      }
    }
    _sth.SpeakFAF(tbxNew.Text);
  }

  void OnWindowRestoree(object s, RoutedEventArgs e) { wr.Visibility = Visibility.Collapsed; wm.Visibility = Visibility.Visible; WindowState = WindowState.Normal; }
  void OnWindowMaximize(object s, RoutedEventArgs e) { wm.Visibility = Visibility.Collapsed; wr.Visibility = Visibility.Visible; WindowState = WindowState.Maximized; }
  void OnWindowClose(object s, RoutedEventArgs e) { Close(); }
}
/*
declare @ms as int, @ia as money
set @ms = 8

SELECT (SELECT BalAmt FROM BalAmtHist WHERE (TxMoneySrcId = @ms) GROUP BY BalAmt, AsOfDate HAVING (AsOfDate = (SELECT MIN(AsOfDate) FROM BalAmtHist WHERE (TxMoneySrcId = @ms)))) AS _1stBalAmt              , SUM(TxAmount) AS TotalBefore1stBalAmt,
     (SELECT BalAmt FROM BalAmtHist WHERE (TxMoneySrcId = @ms) GROUP BY BalAmt, AsOfDate HAVING (AsOfDate = (SELECT MIN(AsOfDate) FROM BalAmtHist WHERE (TxMoneySrcId = @ms))) ) - SUM(TxAmount) AS Diff
FROM TxCoreV2 WHERE (TxDate <= (SELECT MIN(AsOfDate) FROM BalAmtHist WHERE TxMoneySrcId = @ms)) GROUP BY TxMoneySrcId HAVING (TxMoneySrcId = @ms)

DECLARE MY_CURSOR CURSOR  LOCAL STATIC READ_ONLY FORWARD_ONLY FOR  SELECT Id FROM            TxMoneySrc
OPEN MY_CURSOR
FETCH NEXT FROM MY_CURSOR INTO @ms
WHILE @@FETCH_STATUS = 0
BEGIN 
	UPDATE TxMoneySrc SET IniBalance = -(
	SELECT (SELECT BalAmt FROM BalAmtHist WHERE (TxMoneySrcId = @ms) GROUP BY BalAmt, AsOfDate HAVING (AsOfDate = (SELECT MIN(AsOfDate) FROM BalAmtHist WHERE (TxMoneySrcId = @ms))) ) - SUM(TxAmount) 
	FROM TxCoreV2 WHERE (TxDate <= (SELECT MIN(AsOfDate) FROM BalAmtHist WHERE TxMoneySrcId = @ms)) GROUP BY TxMoneySrcId HAVING (TxMoneySrcId = @ms) ) WHERE (Id = @ms)
FETCH NEXT FROM MY_CURSOR INTO @ms
END
CLOSE MY_CURSOR
DEALLOCATE MY_CURSOR

SELECT        Id, CreatedAt, Name, Notes, Fla, IniBalance FROM            TxMoneySrc









USE [FinDemo]
GO

/****** Object:  View [dbo].[Vw_Dupes]    Script Date: 2015-01-20 08:35:01 ****** /
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[Vw_Dupes]
AS
SELECT        core.Id, core.TxMoneySrcId, core.TxCategoryId, core.TxCategoryIdTxt, core.TxSupplierId, core.TxEnvelopeId, core.TxAmount, core.TxDate, core.ProductService, 
                       core.CreatedAt, core.ModifiedAt, core.Notes, core.History, core.TxAmountOrg
FROM            (SELECT        TxAmount, TxDate
                        FROM            dbo.TxCoreV2
                        GROUP BY TxAmount, TxDate
                        HAVING         (COUNT(*) > 1)) AS derivedTbl INNER JOIN
                       dbo.TxCoreV2 AS core ON derivedTbl.TxAmount = core.TxAmount AND derivedTbl.TxDate = core.TxDate

GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
 Begin PaneConfigurations = 
    Begin PaneConfiguration = 0
       NumPanes = 4
       Configuration = "(H (1[40] 4[20] 2[20] 3) )"
    End
    Begin PaneConfiguration = 1
       NumPanes = 3
       Configuration = "(H (1 [50] 4 [25] 3))"
    End
    Begin PaneConfiguration = 2
       NumPanes = 3
       Configuration = "(H (1 [50] 2 [25] 3))"
    End
    Begin PaneConfiguration = 3
       NumPanes = 3
       Configuration = "(H (4[30] 2[40] 3) )"
    End
    Begin PaneConfiguration = 4
       NumPanes = 2
       Configuration = "(H (1 [56] 3))"
    End
    Begin PaneConfiguration = 5
       NumPanes = 2
       Configuration = "(H (2[66] 3) )"
    End
    Begin PaneConfiguration = 6
       NumPanes = 2
       Configuration = "(H (4 [50] 3))"
    End
    Begin PaneConfiguration = 7
       NumPanes = 1
       Configuration = "(V (3))"
    End
    Begin PaneConfiguration = 8
       NumPanes = 3
       Configuration = "(H (1[56] 4[18] 2) )"
    End
    Begin PaneConfiguration = 9
       NumPanes = 2
       Configuration = "(H (1 [75] 4))"
    End
    Begin PaneConfiguration = 10
       NumPanes = 2
       Configuration = "(H (1[66] 2) )"
    End
    Begin PaneConfiguration = 11
       NumPanes = 2
       Configuration = "(H (4 [60] 2))"
    End
    Begin PaneConfiguration = 12
       NumPanes = 1
       Configuration = "(H (1) )"
    End
    Begin PaneConfiguration = 13
       NumPanes = 1
       Configuration = "(V (4))"
    End
    Begin PaneConfiguration = 14
       NumPanes = 1
       Configuration = "(V (2) )"
    End
    ActivePaneConfig = 5
 End
 Begin DiagramPane = 
    PaneHidden = 
    Begin Origin = 
       Top = 0
       Left = 0
    End
    Begin Tables = 
       Begin Table = "derivedTbl"
          Begin Extent = 
             Top = 6
             Left = 38
             Bottom = 101
             Right = 208
          End
          DisplayFlags = 280
          TopColumn = 0
       End
       Begin Table = "core"
          Begin Extent = 
             Top = 6
             Left = 246
             Bottom = 135
             Right = 421
          End
          DisplayFlags = 280
          TopColumn = 0
       End
    End
 End
 Begin SQLPane = 
 End
 Begin DataPane = 
    Begin ParameterDefaults = ""
    End
    Begin ColumnWidths = 9
       Width = 284
       Width = 1500
       Width = 1500
       Width = 1500
       Width = 1500
       Width = 1500
       Width = 1500
       Width = 1500
       Width = 1500
    End
 End
 Begin CriteriaPane = 
    PaneHidden = 
    Begin ColumnWidths = 11
       Column = 1440
       Alias = 900
       Table = 1170
       Output = 720
       Append = 1400
       NewValue = 1170
       SortType = 1350
       SortOrder = 1410
       GroupBy = 1350
       Filter = 1350
       Or = 1350
       Or = 1350
       Or = 1350
    End
 End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Vw_Dupes'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Vw_Dupes'
GO


* 
* 
* 
* 
* 
* 
* 
* 
* 
* 
* 
* 
* 
* 
* 
* 
USE [FinDemoDbg]
GO

/****** Object:  View [dbo].[Vw_Dupes_Detail]    Script Date: 2015-01-20 08:52:08 ****** /
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[Vw_Dupes_Detail]
AS
SELECT        TOP (100) PERCENT core.TxAmount, core.TxDate, core.TxCategoryIdTxt, core.ProductService, core.Notes, dbo.TxSupplier.Name AS Supplier, 
                       dbo.TxMoneySrc.Name AS MoneySrc, core.History, core.CreatedAt, core.ModifiedAt
FROM            (SELECT        TxAmount, TxDate
                        FROM            dbo.TxCoreV2
                        GROUP BY TxAmount, TxDate
                        HAVING         (COUNT(*) > 1)) AS derivedTbl INNER JOIN
                       dbo.TxCoreV2 AS core ON derivedTbl.TxAmount = core.TxAmount AND derivedTbl.TxDate = core.TxDate INNER JOIN
                       dbo.TxSupplier ON core.TxSupplierId = dbo.TxSupplier.Id INNER JOIN
                       dbo.TxMoneySrc ON core.TxMoneySrcId = dbo.TxMoneySrc.Id
ORDER BY core.TxDate DESC, core.TxAmount DESC

GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
 Begin PaneConfigurations = 
    Begin PaneConfiguration = 0
       NumPanes = 4
       Configuration = "(H (1[40] 4[20] 2[20] 3) )"
    End
    Begin PaneConfiguration = 1
       NumPanes = 3
       Configuration = "(H (1 [50] 4 [25] 3))"
    End
    Begin PaneConfiguration = 2
       NumPanes = 3
       Configuration = "(H (1 [50] 2 [25] 3))"
    End
    Begin PaneConfiguration = 3
       NumPanes = 3
       Configuration = "(H (4 [30] 2 [40] 3))"
    End
    Begin PaneConfiguration = 4
       NumPanes = 2
       Configuration = "(H (1 [56] 3))"
    End
    Begin PaneConfiguration = 5
       NumPanes = 2
       Configuration = "(H (2 [66] 3))"
    End
    Begin PaneConfiguration = 6
       NumPanes = 2
       Configuration = "(H (4 [50] 3))"
    End
    Begin PaneConfiguration = 7
       NumPanes = 1
       Configuration = "(V (3))"
    End
    Begin PaneConfiguration = 8
       NumPanes = 3
       Configuration = "(H (1[56] 4[18] 2) )"
    End
    Begin PaneConfiguration = 9
       NumPanes = 2
       Configuration = "(H (1 [75] 4))"
    End
    Begin PaneConfiguration = 10
       NumPanes = 2
       Configuration = "(H (1[66] 2) )"
    End
    Begin PaneConfiguration = 11
       NumPanes = 2
       Configuration = "(H (4 [60] 2))"
    End
    Begin PaneConfiguration = 12
       NumPanes = 1
       Configuration = "(H (1) )"
    End
    Begin PaneConfiguration = 13
       NumPanes = 1
       Configuration = "(V (4))"
    End
    Begin PaneConfiguration = 14
       NumPanes = 1
       Configuration = "(V (2))"
    End
    ActivePaneConfig = 0
 End
 Begin DiagramPane = 
    Begin Origin = 
       Top = 0
       Left = 0
    End
    Begin Tables = 
       Begin Table = "derivedTbl"
          Begin Extent = 
             Top = 6
             Left = 38
             Bottom = 101
             Right = 208
          End
          DisplayFlags = 280
          TopColumn = 0
       End
       Begin Table = "core"
          Begin Extent = 
             Top = 6
             Left = 246
             Bottom = 135
             Right = 421
          End
          DisplayFlags = 280
          TopColumn = 0
       End
       Begin Table = "TxSupplier"
          Begin Extent = 
             Top = 6
             Left = 459
             Bottom = 135
             Right = 629
          End
          DisplayFlags = 280
          TopColumn = 0
       End
       Begin Table = "TxMoneySrc"
          Begin Extent = 
             Top = 6
             Left = 667
             Bottom = 135
             Right = 837
          End
          DisplayFlags = 280
          TopColumn = 0
       End
    End
 End
 Begin SQLPane = 
 End
 Begin DataPane = 
    Begin ParameterDefaults = ""
    End
    Begin ColumnWidths = 9
       Width = 284
       Width = 1500
       Width = 1500
       Width = 1500
       Width = 1500
       Width = 1500
       Width = 1500
       Width = 1500
       Width = 1500
    End
 End
 Begin CriteriaPane = 
    Begin ColumnWidths = 11
       Column = 1440
       Alias = 900
       Table = 1170
       Output = 720
       Append = 1400
       NewValue = 1170
       SortType = 1350
       SortOrder = 1410
       GroupBy = 1350
       Filter = 1350
       Or = 1350
       Or = 1350
       Or = 1350
    End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Vw_Dupes_Detail'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'
 End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Vw_Dupes_Detail'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Vw_Dupes_Detail'
GO

647-296-8492 mei's phone for the reports.

* 
* 
*/

public static class DbSaveMsgBox
{
  public static int CheckAskSave(DbContext db, int maxRowsToShow = 32, MessageBoxButton btn = MessageBoxButton.YesNoCancel)
  {
    var rowsSaved = (int)MsgBoxDbRslt.Unknown;
  retry:
    try
    {
      //_bpr.Ok();

      if (!db.ChangeTracker.Entries().Any(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted))
      {
        rowsSaved = (int)MsgBoxDbRslt.NoChanges;
      }
      else
      {
        switch (MessageBox.Show(db.GetDbChangesReport(maxRowsToShow), "Save Changes?", btn, MessageBoxImage.Question, MessageBoxResult.Yes))
        {
          case MessageBoxResult.Yes: rowsSaved = TrySaveAsk(db, "Manual Yes"); if (rowsSaved < 0) goto retry; return rowsSaved; // >0;		yes
          case MessageBoxResult.No: return (int)MsgBoxDbRslt.No;                                                  // -1;		no
          case MessageBoxResult.Cancel: return (int)MsgBoxDbRslt.Cancel;                                          // -2;		cancel
        }
      }
    }
    catch (Exception ex)
    {
      _ = ex.Log();
      if (MessageBox.Show(ex.ToString(), "Exception detected - Retry saving?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
        goto retry;
    }

    return rowsSaved;
  }
  public static int TrySaveAsk(DbContext db, string note)
  {
  retry:
    try
    {
      var sw = Stopwatch.StartNew();
      var rowsSaved = db.SaveChanges();
      var rv = $"{rowsSaved,8:N0} rows saved in{sw.Elapsed.TotalSeconds,6:N1}s  <=  {note}";
      Trace.WriteLine(rv);
      return rowsSaved;
    }
    catch (DbEntityValidationException ex)
    {
      var ves = ex.ValidationExceptionToString();
      Trace.WriteLine(ves);
      if (Debugger.IsAttached) Debugger.Break();
      else if (MessageBox.Show(ves, "Exceptions detected - Retry saving?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
        goto retry;
    }
    catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
    {
      static string DbUpdateExceptionToString(Microsoft.EntityFrameworkCore.DbUpdateException ex)
      {
        var sb = new StringBuilder();

        foreach (var er in ex.Entries)
        {
          _ = sb.Append($"\r\n:>DbUpdateException:  {ex.InnerMessages()}\t");
        }

        return sb.ToString();
      }

      var ves = DbUpdateExceptionToString(ex);
      Trace.WriteLine(ves);
      if (Debugger.IsAttached) Debugger.Break();
      else if (MessageBox.Show(ves, "Exceptions detected - Retry saving?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
        goto retry;
    }
    catch (Exception ex)
    {
      Trace.WriteLine(ex.InnermostMessage());
      if (Debugger.IsAttached) Debugger.Break();
      if (MessageBox.Show(ex.InnermostMessage(), "Exception detected - Retry saving?", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.Yes) == MessageBoxResult.Yes)
        goto retry;
    }

    return -2;
  }
}

public enum MsgBoxDbRslt // MsgBoxReverseRslt
{
  OK = -MessageBoxResult.OK,         // -1
  No = -MessageBoxResult.No,         // -7
  Yes = -MessageBoxResult.Yes,       // -6
  Cancel = -MessageBoxResult.Cancel, // -2
  Unknown = -3,
  NoChanges = -4,
}
//public static class DbxExt // replacing DbSaveLib and all others!!! (Aug 2018  ...2021-10  ...2022-05)
//{
//  public static async Task<(bool success, int rowsSavedCnt, string report)> TrySaveReportAsync(this DbContext dbx, string? info = "", [CallerMemberName] string callerName = "")
//  {
//    var report = $"■■ {info}.{callerName}()  rows saved: ";

//    try
//    {
//      var stopwatch = Stopwatch.StartNew();
//      var rowsSaved = await dbx.SaveChangesAsync();

//      report += stopwatch.ElapsedMilliseconds < 250 ? $"{rowsSaved,7:N0} ■■" : $"{rowsSaved,7:N0} / {VersionHelper.TimeAgo(stopwatch.Elapsed, small: true)} => {rowsSaved / stopwatch.Elapsed.TotalSeconds:N0} rps ■■";

//      WriteLine(report);

//      return (true, rowsSaved, report);
//    }
//    catch (DbEntityValidationException ex)                          /**/ { report += ex.Log($"\n{ValidationExceptionToString(ex)}"); }
//    catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)      /**/ { report += ex.Log($"\n{string.Join("\t", ex.Entries.Select(r => r.ToString()))}  [{ex.Entries.Count,5} rows affected]  :Core"); }
//    catch (System.Data.Entity.Infrastructure.DbUpdateException ex)  /**/ { report += ex.Log($"\n{string.Join("\t", ex.Entries.Select(r => r.ToString()))}  [{ex.Entries.Count()} rows affected]  :Infr"); }
//    catch (Exception ex)                                            /**/ { report += ex.Log(); }

//    return (false, -88, report);
//  }
//  public static void DiscardChanges(this DbContext _dba) => _dba.ChangeTracker.Clear();
//  public static bool HasUnsavedChanges(this DbContext _dba) => _dba != null && _dba.ChangeTracker.Entries().Any(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted);
//  public static string GetDbChangesReport(this DbContext _dba, int maxLinesToShow = 33)
//  {
//    var sb = new StringBuilder($"{_dba.GetType().Name}.{_dba.GetType().GetHashCode()}:  {_dba.ChangeTracker.Entries().Count(e => e.State == EntityState.Deleted),5} Del  {_dba.ChangeTracker.Entries().Count(e => e.State == EntityState.Added),5} Ins  {_dba.ChangeTracker.Entries().Count(e => e.State == EntityState.Modified),5} Upd");

//    var lineCounter = 0;
//    foreach (var modifieds in _dba.ChangeTracker.Entries().Where(e => e.State == EntityState.Modified))
//    {
//      foreach (var pn in modifieds.CurrentValues.Properties)
//      {
//        var org = modifieds.OriginalValues[pn];
//        var cur = modifieds.CurrentValues[pn];

//        if (!Equals(modifieds.CurrentValues[pn], org))
//        {
//          if (++lineCounter <= maxLinesToShow)
//          {
//            _ = sb.Append($"\n{pn?.ToString()?.Replace("Property: ", ""),-17}  \t{SafeValue(org)} → {SafeValue(cur)}");
//          }
//          else
//          {
//            _ = sb.Append(" ...");
//            break;
//          }
//        }
//      }

//      if (lineCounter > maxLinesToShow) break;
//    }

//    return sb.ToString();
//  }

//  public static string ValidationExceptionToString(this DbEntityValidationException ex)
//  {
//    var sb = new StringBuilder();

//    foreach (var eve in ex.EntityValidationErrors)
//    {
//      _ = sb.AppendLine($"""- Entity of type "{eve.Entry.Entity.GetType().FullName}" in state "{eve.Entry.State}" has the following validation errors:""");
//      foreach (var ve in eve.ValidationErrors)
//      {
//        object value;
//        if (ve.PropertyName.Contains('.'))
//        {
//          var propertyChain = ve.PropertyName.Split('.');
//          var complexProperty = eve.Entry.CurrentValues.GetValue<DbPropertyValues>(propertyChain.First());
//          value = GetComplexPropertyValue(complexProperty, propertyChain.Skip(1).ToArray());
//        }
//        else
//        {
//          value = eve.Entry.CurrentValues.GetValue<object>(ve.PropertyName);
//        }

//        _ = sb.AppendLine($"-- Property: \"{ve.PropertyName}\", Value: \"{value}\", Error: \"{ve.ErrorMessage}\"");
//      }
//    }

//    const int maxCombinedErrorMessageLength = 4000;
//    return sb.Length < maxCombinedErrorMessageLength ? sb.ToString() : (sb.ToString()[..maxCombinedErrorMessageLength] + " ...");
//  }
//  public static string ServerDatabase(this Microsoft.EntityFrameworkCore.DbContext dbx)
//  {
//    var constr = dbx.Database.GetConnectionString() ?? "";
//    var kvpLst = constr.Split(';').ToList();
//    return
//      GetConStrValue(kvpLst, "data source") ??
//      GetConStrValue(kvpLst, "server") ??
//      $"No server name found in the con. string  {constr} :(";
//    //return $"{(server.Equals(@"(localdb)\MSSQLLocalDB", StringComparison.OrdinalIgnoreCase) ? "" : server.Contains("database.windows.net") ? "Azure\\" : server)}{getConStrValue(kvpList, "AttachDbFilename")}{getConStrValue(kvpList, "initial catalog")}";
//  }
//  public static string SqlConStrValues(this string constr, int firstN = 10) => string.Join("·", constr.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList().Take(firstN).Select(r => r.Split('=').LastOrDefault() ?? "°"));

//  static string? GetConStrValue(List<string> lst, string key) => lst.FirstOrDefault(r => r.Split('=')[0].Equals(key, StringComparison.OrdinalIgnoreCase))?.Split('=')[1];
//  static object GetComplexPropertyValue(DbPropertyValues propertyValues, string[] propertyChain)
//  {
//    var propertyName = propertyChain.First();

//    return propertyChain.Length == 1
//        ? propertyValues[propertyName]
//        : GetComplexPropertyValue((DbPropertyValues)propertyValues[propertyName], propertyChain.Skip(1).ToArray());
//  }
//  static string SafeValue(object? val) => val is string str ?
//    str.Length <= _maxWidth ? str : $"\r\n  {str[.._maxWidth].Replace("\n", " ").Replace("\r", " ")}...{str.Length:N0}\r\n" :
//    val?.ToString() ?? "Null";

//  const int _maxWidth = 42;
//}