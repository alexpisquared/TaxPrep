namespace MF.TxCategoryAssigner;
public partial class TxCategoryAssignerVw : WindowBase
{
  readonly FinDemoContext _db = new();
  readonly DateTime _yrStart2004 = new(2004, 1, 1), _now = DateTime.Now;
  readonly ObservableCollection<TxCoreV2> _core = new();
  readonly ObservableCollection<TxCategory> _catg = new();
  readonly CollectionViewSource _txnCtg, _txCoreV2_Root_VwSrc;
  readonly int _trgTaxYr = DateTime.Today.Year - 1;
  readonly ILogger<TxCategoryAssignerVw> _lgr;
  readonly Bpr _bpr;
  string? _txCatgry, _loadedCatgry = "?", _choiceAbove, _choiceBelow;
  decimal _selectTtl = 0;
  bool _loaded = false;
  int? _cutOffYr = null;

  public TxCategoryAssignerVw(ILogger<TxCategoryAssignerVw> lgr, Bpr bpr)
  {
    InitializeComponent();
    _txnCtg = (CollectionViewSource)FindResource("txCategoryVwSrcDatGrd");
    _txCoreV2_Root_VwSrc = (CollectionViewSource)FindResource("txCoreV2_Root_VwSrc");
    _lgr = lgr;
    _bpr = bpr;
  }
  async void OnLoaded(object s, RoutedEventArgs e)
  {
    App.Synth.SpeakExpressFAF("Loading...");

    try
    {
      await _db.TxCategories.LoadAsync();
      await _db.TxMoneySrcs.LoadAsync();

      _txnCtg.Source = _db.TxCategories.Local.OrderBy(r => r.ExpGroup?.Name).ThenBy(r => r.TlNumber);
      ((CollectionViewSource)FindResource("txCategoryVwSrcComBox")).Source = _db.TxCategories.Local.OrderBy(r => r.Name).ThenBy(r => r.TlNumber);

      _ = tbxSearch.Focus();
      var y = DateTime.Today.Year;
      cbxSingleYr.ItemsSource = new int[] { y, y - 1, y - 2, y - 3, y - 4, y - 5, y - 6, y - 7, y - 8, y - 9 };
      cbxSingleYr.SelectedIndex = 2;
      chkInfer.IsChecked = true;

      btAssign.IsEnabled = btAssig2.IsEnabled = false;

      _loaded = true;
      chkSingleYr.IsChecked = true;      // invokes the 1st search

      App.Synth.SpeakExpressFAF("Done!");
    }
    catch (Exception ex) { ex.Pop(); }
  }
  protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
  {
    if (_db.HasUnsavedChanges())
    {
      switch (MessageBox.Show($"Would you like to save the changes? \r\n\n{_db.GetDbChangesReport()}", "Unsaved changes detected", MessageBoxButton.YesNoCancel, MessageBoxImage.Question))
      {
        default:
        case MessageBoxResult.Cancel: e.Cancel = true; return;
        case MessageBoxResult.Yes: var (success, rowsSavedCnt, report) = _db.TrySaveReportAsync().Result; if (!success) _ = MessageBox.Show(report, $"Attach to process!!!  ({rowsSavedCnt})"); break;
        case MessageBoxResult.No: break;
      }
    }

    //if (_dispose)
    _db.Dispose();

    base.OnClosing(e);
  }

  async Task FilterStart(string csvFilterString)
  {
    if (!_loaded) return;

    var started = Stopwatch.GetTimestamp();

    try
    {
      WriteLine($"■■■ filterStart()  [[");

      _bpr.Beep(20_000, .05); // wake monitor speakers

      await reLoadTxCore();

      if (string.IsNullOrEmpty(csvFilterString))
      {
        App.Synth.SpeakExpressFAF("Clear!");
        filterTxns(csvFilterString, _txCatgry);
      }
      else
      {
        var ta = csvFilterString.Split(new[] { '`', '>', '\\', '/' });
        if (ta.Length > 1)
        {
          App.Synth.SpeakExpressFAF($"{ta.Length}-part filter");

          if (string.IsNullOrEmpty(ta[0]) && string.IsNullOrEmpty(ta[1])) { App.Synth.SpeakExpressFAF("Still Empty."); return; }

          if (!string.IsNullOrEmpty(ta[0]))
          {
            if (!decimal.TryParse(ta[0], out var amt)) { App.Synth.SpeakExpressFAF("1st must be number."); return; }

            if (!decimal.TryParse(tRng.Text, out var rng)) tRng.Text = (rng = 0m).ToString();
            //App.Synth.SpeakExpressFAF("Multi.");
            filterTxns(csvFilterString[(ta[0].Length + 1)..], _txCatgry, amt, rng);
          }
          else if (!string.IsNullOrEmpty(ta[1])) // explicit by string
          {
            //App.Synth.SpeakExpressFAF("Filter by text.");
            filterTxns(ta[1], _txCatgry);
          }
        }
        else // == 1
        {
          if (decimal.TryParse(csvFilterString, out var amt))
          {
            if (!decimal.TryParse(tRng.Text, out var rng)) tRng.Text = (rng = 0m).ToString();
            //App.Synth.SpeakExpressFAF("Filter by money.");
            filterTxns("", _txCatgry, amt, rng);
          }
          else
          {
            //App.Synth.SpeakExpressFAF("Filter by text.");
            filterTxns(csvFilterString, _txCatgry);
          }
        }
      }

      _txCoreV2_Root_VwSrc.Source = _core;

      if (chkInfer.IsChecked == true)
      {
        var catIds = _core.Select(r => r.TxCategoryIdTxtNavigation.Id).Distinct().Where(r => r != 3); // Debug.WriteLine($"catIds.Count() = {catIds.Count()}");
        filterCategoryByIdList(catIds);
      }

      UpdateTitle(Stopwatch.GetElapsedTime(started));
    }
    catch (Exception ex) { ex.Pop(); }
    finally { WriteLine($"■■■ filterStart()  ]]"); _bpr.Tick(); }
  }
  async Task reLoadTxCore() // limit txns by chkboxs' filters
  {
    if (!_loaded) return;

    Debug.WriteLine($"■■■   reLoadTxCore()");

    try
    {
      if (chkTxCatgry.IsChecked == true && chkSingleYr.IsChecked == true)
      {
        await _db.TxCoreV2s.Where(r => r.TxDate.Year >= _cutOffYr && (string.Compare(r.TxCategoryIdTxt, _txCatgry, true) == 0)).LoadAsync();
        _loadedCatgry = _txCatgry;
      }
      else if (chkTxCatgry.IsChecked == true)
      {
        await _db.TxCoreV2s.Where(r => r.TxDate >= _yrStart2004 && (string.Compare(r.TxCategoryIdTxt, _txCatgry, true) == 0)).LoadAsync();
        _loadedCatgry = _txCatgry;
      }
      else if (chkSingleYr.IsChecked == true)
      {
        await _db.TxCoreV2s.Where(r => r.TxDate.Year >= _cutOffYr).LoadAsync();
      }
      else
      {
        await _db.TxCoreV2s.Where(r => r.TxDate >= _yrStart2004).LoadAsync();
        _loadedCatgry = _txCatgry = null;
      }

      if (dgTxCore.ItemsSource != null)
      {
        CollectionViewSource.GetDefaultView(dgTxCore.ItemsSource).Refresh(); //tu:            
        _bpr.Tick();
      }
      else
      {
        _bpr.Tick();
      }
    }
    catch (Exception ex) { ex.Pop(); }
  }
  void filterTxns(string strFilter, string? txCatgoryId)
  {
    tbkFlt.Text = strFilter;
    tbkAmt.Text = "";

    _core.ClearAddRangeAuto(_db.TxCoreV2s.Local.Where(r => (
      (!string.IsNullOrEmpty(r.TxDetail) && r.TxDetail.ToLower().Contains(strFilter.ToLower())) ||
      (!string.IsNullOrEmpty(r.MemoPp) && r.MemoPp.ToLower().Contains(strFilter.ToLower())) ||
      (!string.IsNullOrEmpty(r.Notes) && r.Notes.ToLower().Contains(strFilter.ToLower())))
      &&
      (_cutOffYr == null ? r.TxDate >= _yrStart2004 : r.TxDate.Year >= _cutOffYr)
      &&
      (
        string.IsNullOrEmpty(txCatgoryId) ||

            !string.IsNullOrEmpty(strFilter) ||
             (r.TxDate.Year >= _trgTaxYr && string.Compare(r.TxCategoryIdTxt, txCatgoryId, true) == 0)
             ||
          (r.TxDate.Year < _trgTaxYr)

      )
    ).OrderByDescending(r => r.TxDate));
  }
  void filterTxns(string strFilter, string? txCatgoryId, decimal amt, decimal rng)
  {
    tbkFlt.Text = strFilter;
    tbkAmt.Text = $"{amt:N0}";

    _core.ClearAddRangeAuto(_db.TxCoreV2s.Local.Where(r =>
      amt - rng <= (chkIsAbs.IsChecked == true ? Math.Abs(r.TxAmount) : r.TxAmount) && (chkIsAbs.IsChecked == true ? Math.Abs(r.TxAmount) : r.TxAmount) <= amt + rng
       && (
      (!string.IsNullOrEmpty(r.TxDetail) && r.TxDetail.ToLower().Contains(strFilter.ToLower())) ||
      (!string.IsNullOrEmpty(r.MemoPp) && r.MemoPp.ToLower().Contains(strFilter.ToLower())) ||
      (!string.IsNullOrEmpty(r.Notes) && r.Notes.ToLower().Contains(strFilter.ToLower())))
      &&
      (_cutOffYr == null ? r.TxDate >= _yrStart2004 : r.TxDate.Year >= _cutOffYr)
      &&
      (string.IsNullOrEmpty(txCatgoryId) || string.Compare(r.TxCategoryIdTxt, txCatgoryId, true) == 0)).OrderByDescending(r => r.TxDate));
  }
  void filterCategoryByIdList(IEnumerable<int> catIds) => updateCtgrList(_db.TxCategories.Local.Where(r => catIds.Contains(r.Id)));
  void filterCategoryByTxtMatch(string namePart/**/  ) => updateCtgrList(_db.TxCategories.Local.Where(r => r.Name.ToLower().Contains(namePart) || r.IdTxt.ToLower().Contains(namePart)));

  void updateCtgrList(IEnumerable<TxCategory> enu)
  {
    var started = Stopwatch.GetTimestamp();

    _catg.ClearAddRangeAuto(enu);

    ArgumentNullException.ThrowIfNull(_txnCtg);
    _txnCtg.Source = _catg;

    if (_catg.Count() == 0)
    {
      _catg.ClearAddRangeAuto(_db.TxCategories.Local.OrderBy(r => r.ExpGroup?.Name).ThenBy(r => r.TlNumber));
    }

    choiceAbove.IsEnabled = choiceBelow.IsEnabled = btAssign.IsEnabled = btAssig2.IsEnabled = false;
    _choiceAbove = _choiceBelow = "";

    if (_catg.Count() == 1)
    {
      btAssign.IsEnabled = btAssig2.IsEnabled = true;
      _ = _txnCtg.View.MoveCurrentToFirst();
    }
    else if (_catg.Count() == 2)
    {
      _choiceAbove = _catg.First().IdTxt;
      _choiceBelow = _catg.Last().IdTxt;
      btAssign.IsEnabled = btAssig2.IsEnabled = txCategoryListBox.SelectedItems.Count == 1;// _choiceAbove == _choiceBelow;
    }

    choiceAbove.Content = $"_1 {_choiceAbove}"; choiceAbove.IsEnabled = _choiceAbove.Length > 0;
    choiceBelow.Content = $"_7 {_choiceBelow}"; choiceBelow.IsEnabled = _choiceBelow.Length > 0;

    UpdateTitle(Stopwatch.GetElapsedTime(started));
  }

  async Task<bool> isStillTyping(TextBox textbox) { var tx = textbox.Text.ToLower(); await Task.Delay(750); return tx != textbox.Text.ToLower(); }
  void UpdateTitle(TimeSpan timeSpan, [CallerMemberName] string? cmn = "") => WriteLine(Title = $"Ctgry {_loadedCatgry,-12} {_core.Count(),9:N0} rows    sum {_core.Where(r => r.TxCategoryIdTxt == "UnKn").Sum(r => r.TxAmount),9:N0} / {_core.Sum(r => r.TxAmount),-9:N0}    selects {_selectTtl,9:N2}  {timeSpan.TotalSeconds,4:N1}s   {cmn}");

  async void onReLoad(object s, RoutedEventArgs e)
  {
    switch (DbSaveMsgBox.CheckAskSave(_db))
    {
      case (int)MsgBoxDbRslt.Yes: break;
      default: break;
      case (int)MsgBoxDbRslt.Cancel: return;
      case (int)MsgBoxDbRslt.No: break;
    }

    await reLoadTxCore();
    onClear1(s, e);
  }
  void onReLoad2(object s, RoutedEventArgs e) => MessageBox.Show("Not implemented!!!!!!!\n\nSee .Net 4.8 version for details.", "ImportToDB.DoAll()");

  async void onTextChangedFuz(object s, TextChangedEventArgs e) { if (!await isStillTyping((TextBox)s)) await FilterStart(tbxSearch.Text); }
  async void onTextChangedCtg(object s, TextChangedEventArgs e) { if (!await isStillTyping((TextBox)s)) filterCategoryByTxtMatch(((TextBox)s).Text); }

  void onClear1(object s, RoutedEventArgs e) => tbkFlt.Text = tbxSearch.Text = "";
  void onClear2(object s, RoutedEventArgs e) => tSrch2.Text = "";

  void dgTxCtgr_SelectionChanged(object s, SelectionChangedEventArgs e) => btAssign.IsEnabled = btAssig2.IsEnabled = e.AddedItems.Count == 1;
  void dgTxCore_SelectionChanged(object s, SelectionChangedEventArgs e)
  {
    try
    {
      var started = Stopwatch.GetTimestamp();

      _choiceAbove = _choiceBelow = "";

      _selectTtl = 0;
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
        }
        else
        {
          var trg = lv.Where(r => r.TxCategoryIdTxtNavigation.IdTxt != "UnKn")?.FirstOrDefault();
          _choiceAbove = _choiceBelow = trg?.TxCategoryIdTxtNavigation.IdTxt ?? "";
          btAssign.IsEnabled = btAssig2.IsEnabled = txCategoryListBox.SelectedItems.Count == 1;
        }
      }

      choiceAbove.Content = $"_1 {_choiceAbove}"; choiceAbove.IsEnabled = _choiceAbove.Length > 0;
      choiceBelow.Content = $"_7 {_choiceBelow}"; choiceBelow.IsEnabled = _choiceBelow.Length > 0;

      UpdateTitle(Stopwatch.GetElapsedTime(started));
    }
    catch (Exception ex) { ex.Pop(); }
  }

  void /**/  onTxCatgry_Checked(object s, RoutedEventArgs e) { _ = cbxTxCatgry.Focus(); onTxCatgry_Changed(s, (SelectionChangedEventArgs)e); }
  async void onTxCatgry_UnChckd(object s, RoutedEventArgs e)           /**/ { _txCatgry = null;                                 /**/ await FilterStart(tbxSearch.Text); }
  async void onTxCatgry_Changed(object s, SelectionChangedEventArgs e) /**/ { if (!_loaded) return; _txCatgry = (string)cbxTxCatgry.SelectedValue;    /**/ await FilterStart(tbxSearch.Text); }

  async void onSingleYr_Checked(object s, RoutedEventArgs e)           /**/ { _ = (cbxSingleYr?.Focus()); await SingleYr_ChangedTask(); }
  async void onSingleYr_UnChckd(object s, RoutedEventArgs e)           /**/ { _cutOffYr = null;                                 /**/ await FilterStart(tbxSearch.Text); }
  async void onSingleYr_Changed(object s, SelectionChangedEventArgs e) /**/ => await SingleYr_ChangedTask(); async Task SingleYr_ChangedTask() { _cutOffYr = (int)cbxSingleYr.SelectedValue;       /**/ await FilterStart(tbxSearch.Text); }

  void OnLbxSelectionChanged(object s, SelectionChangedEventArgs e) { if (e.AddedItems.Count < 1) return; var started = Stopwatch.GetTimestamp(); UpdateTitle(Stopwatch.GetElapsedTime(started)); Title += string.Format(" - {0}", ((TxCategory?)txCategoryListBox.SelectedItems[0])?.Name); }
  async void onInfer_Checked(object s, RoutedEventArgs e) => await FilterStart(tbkFlt.Text);
  void dgTxCtgr_MouseDblClick(object s, MouseButtonEventArgs e) { if (btAssign.IsEnabled) onAssign0(s, e); }
  void dgTxCore_MouseDblClick(object s, MouseButtonEventArgs e)
  {
    if (e.OriginalSource is System.Windows.Documents.Run run)
      Clipboard.SetText(run.Text);
    else
      Clipboard.SetText(((TxCoreV2)((System.Windows.Controls.Primitives.Selector)s).SelectedItem).TxDetail);

    App.Synth.SpeakExpressFAF("Copied!");
  }

  async void onManualTxnAdd(object s, RoutedEventArgs e) { _ = new ManualTxnEntry(_db).ShowDialog(); await reLoadTxCore(); }
  async void onAssign0(object s, RoutedEventArgs e) { if (txCategoryListBox.SelectedItems.Count == 1) await assign(((TxCategory?)txCategoryListBox.SelectedItems[0])?.IdTxt); }
  async void onAssign1(object s, RoutedEventArgs e) => await assign(_choiceAbove);
  async void onAssign2(object s, RoutedEventArgs e) => await assign(_choiceBelow);
  async Task assign(string? IdTxt)
  {
    ArgumentNullException.ThrowIfNull(IdTxt, "▄▀▄▀▄▀▄▀▄▀▄▀");

    btAssign.IsEnabled = btAssig2.IsEnabled = choiceAbove.IsEnabled = choiceBelow.IsEnabled = false;

    if (dgTxCore.SelectedItems.Count > 0)
    {
      foreach (TxCoreV2 tc in dgTxCore.SelectedItems) { tc.TxCategoryIdTxt = IdTxt; tc.ModifiedAt = _now; }
    }
    else if (tbxSearch.Text.Length > 1)
    {
      foreach (var tc in _core)
      {
        if (tc.TxCategoryIdTxt.Equals("UnKn", StringComparison.OrdinalIgnoreCase)) // if (!tc.TxCategoryIdTxt.Equals(IdTxt, StringComparison.OrdinalIgnoreCase))
        {
          tc.TxCategoryIdTxt = IdTxt; tc.ModifiedAt = _now;
        }
      }
    }

    if (DbSaveMsgBox.TrySaveAsk(_db, $"class TxCategoryAssignerVw.assign()") > 0)//== (int)MsgBoxDbRslt.Yes)
    {
      await reLoadTxCore();
      onClear1(default!, default!);
      onClear2(default!, default!);
    }

    _ = tbxSearch.Focus();
  }
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
//    var report = $"■■ {info}.{callerName}()  records saved: ";

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
//  public static void DiscardChanges(this DbContext db) => db.ChangeTracker.Clear();
//  public static bool HasUnsavedChanges(this DbContext db) => db != null && db.ChangeTracker.Entries().Any(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted);
//  public static string GetDbChangesReport(this DbContext db, int maxLinesToShow = 33)
//  {
//    var sb = new StringBuilder($"{db.GetType().Name}.{db.GetType().GetHashCode()}:  {db.ChangeTracker.Entries().Count(e => e.State == EntityState.Deleted),5} Del  {db.ChangeTracker.Entries().Count(e => e.State == EntityState.Added),5} Ins  {db.ChangeTracker.Entries().Count(e => e.State == EntityState.Modified),5} Upd");

//    var lineCounter = 0;
//    foreach (var modifieds in db.ChangeTracker.Entries().Where(e => e.State == EntityState.Modified))
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