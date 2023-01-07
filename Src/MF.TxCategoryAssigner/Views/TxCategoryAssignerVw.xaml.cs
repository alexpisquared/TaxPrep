using AAV.Sys.Helpers;
using AAV.WPF.Ext;
using AsLink;
using Db.FinDemo.DbModel;
using MF.TxCategoryAssigner.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using xMvvmMin;

namespace MF.TxCategoryAssigner
{
  public partial class TxCategoryAssignerVw : AAV.WPF.Base.WindowBase
  {
    readonly A0DbContext _db = A0DbContext.Create();
    readonly DateTime _yrStart2004 = new DateTime(2004, 1, 1), _now = DateTime.Now;
    readonly ObservableCollection<TxCoreV2> _core = new ObservableCollection<TxCoreV2>();
    readonly ObservableCollection<TxCategory> _catg = new ObservableCollection<TxCategory>();
    CollectionViewSource _txnCtg, _txCoreV2_Root_VwSrc;
    string _txCatgry = "UnKn", _loadedCatgry = "?", _choiceAbove, _choiceBelow;
    decimal _selectTtl = 0;
    bool _loaded = false;
    int? _cutOffYr, _trgTaxYr = DateTime.Today.Year - 1;

    public TxCategoryAssignerVw() => InitializeComponent();
    async void onLoaded(object s, RoutedEventArgs e)
    {
      App.Synth.Rate = +7;
      App.Synth.SelectVoiceByHints(VoiceGender.Female);
      App.Synth.SpeakAsync("Loading...");

      await _db.TxCategories.LoadAsync();

      (_txnCtg = (CollectionViewSource)FindResource("txCategoryVwSrcDatGrd")).Source = _db.TxCategories.Local.OrderBy(r => r.ExpenseGroup.Name).ThenBy(r => r.TL_Number);
      ((CollectionViewSource)FindResource("txCategoryVwSrcComBox")).Source = _db.TxCategories.Local.OrderBy(r => r.Name).ThenBy(r => r.TL_Number); // cbxTxCatgry.ItemsSource = _db.TxCategories.OrderBy(r => r.Name).ToList();
      _txCoreV2_Root_VwSrc = ((CollectionViewSource)FindResource("txCoreV2_Root_VwSrc"));

      tbxSearch.Focus();
      var y = DateTime.Today.Year;
      cbxSingleYr.ItemsSource = new int[] { y, y - 1, y - 2, y - 3, y - 4, y - 5, y - 6, y - 7, y - 8, y - 9 };
      cbxSingleYr.SelectedIndex = 2;
      chkInfer.IsChecked = true;

      _txCatgry = null;
      _cutOffYr = null;
      btAssign.IsEnabled = btAssig2.IsEnabled = false;

      _loaded = true;
      chkSingleYr.IsChecked = true;      // invokes the 1st search
      App.Synth.SpeakAsync("Done!");
    }
    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
      if (_db.HasUnsavedChanges())
      {
        switch (MessageBox.Show($"Would you like to save the changes? \r\n\n{_db.GetDbChangesReport()}", "Unsaved changes detected", MessageBoxButton.YesNoCancel, MessageBoxImage.Question))
        {
          default:
          case MessageBoxResult.Cancel: e.Cancel = true; return;
          case MessageBoxResult.Yes: var (success, rowsSavedCnt, report) = _db.TrySaveReport(); if (!success) MessageBox.Show(report, $"Attach to process!!!  ({rowsSavedCnt})"); break;
          case MessageBoxResult.No: break;
        }
      }

      //if (_dispose)
      _db.Dispose();

      base.OnClosing(e);
    }

    async Task filterStart(string csvFilterString)
    {
      if (!_loaded) return;

      Debug.WriteLine($"■■■ filterStart()");

      //App.Synth.SpeakAsyncCancelAll();
      //Bpr.BeepFD(12000, 33); // wake monitor speakers

      await reLoadTxCore();

      if (string.IsNullOrEmpty(csvFilterString))
      {
        App.Synth.SpeakAsync("Clear!");
        filterTxns(csvFilterString, _txCatgry);
      }
      else
      {
        var ta = csvFilterString.Split(new[] { '`', '>', '\\', '/' });
        if (ta.Length > 1)
        {
          App.Synth.SpeakAsync($"{ta.Length}-part filter");

          if (string.IsNullOrEmpty(ta[0]) && string.IsNullOrEmpty(ta[1])) { App.Synth.SpeakAsync("Still Empty."); return; }
          if (!string.IsNullOrEmpty(ta[0]))
          {
            if (!decimal.TryParse(ta[0], out var amt)) { App.Synth.SpeakAsync("1st must be number."); return; }

            if (!decimal.TryParse(tRng.Text, out var rng)) tRng.Text = (rng = 0m).ToString();
            //App.Synth.SpeakAsync("Multi.");
            filterTxns(csvFilterString.Substring(ta[0].Length + 1), _txCatgry, amt, rng);
          }
          else if (!string.IsNullOrEmpty(ta[1])) // explicit by string
          {
            //App.Synth.SpeakAsync("Filter by text.");
            filterTxns(ta[1], _txCatgry);
          }
        }
        else // == 1
        {
          if (decimal.TryParse(csvFilterString, out var amt))
          {
            if (!decimal.TryParse(tRng.Text, out var rng)) tRng.Text = (rng = 0m).ToString();
            //App.Synth.SpeakAsync("Filter by money.");
            filterTxns("", _txCatgry, amt, rng);
          }
          else
          {
            //App.Synth.SpeakAsync("Filter by text.");
            filterTxns(csvFilterString, _txCatgry);
          }
        }
      }

      _txCoreV2_Root_VwSrc.Source = _core;

      if (chkInfer.IsChecked == true)
      {
        var catIds = _core.Select(r => r.TxCategory.Id).Distinct().Where(r => r != 3); // Debug.WriteLine($"catIds.Count() = {catIds.Count()}");
        filterCategoryByIdList(catIds);
      }

      updateTitle();
      //Bpr.OkFaF();
    }
    async Task reLoadTxCore() // limit txns by chkboxs' filters
    {
      if (!_loaded) return;

      Debug.WriteLine($"■■■   reLoadTxCore()");

      try
      {
        if (chkTxCatgry.IsChecked == true && chkSingleYr.IsChecked == true)
        {
          await _db.TxCoreV2.Where(r => r.TxDate.Year >= _cutOffYr && (string.Compare(r.TxCategoryIdTxt, _txCatgry, true) == 0)).LoadAsync();
          _loadedCatgry = _txCatgry;
        }
        else if (chkTxCatgry.IsChecked == true)
        {
          await _db.TxCoreV2.Where(r => r.TxDate >= _yrStart2004 && (string.Compare(r.TxCategoryIdTxt, _txCatgry, true) == 0)).LoadAsync();
          _loadedCatgry = _txCatgry;
        }
        else if (chkSingleYr.IsChecked == true)
        {
          await _db.TxCoreV2.Where(r => r.TxDate.Year >= _cutOffYr).LoadAsync();
        }
        else
        {
          await _db.TxCoreV2.Where(r => r.TxDate >= _yrStart2004).LoadAsync();
          _loadedCatgry = _txCatgry = null;
        }

        if (dgTxCore.ItemsSource != null)
        {
          CollectionViewSource.GetDefaultView(dgTxCore.ItemsSource).Refresh(); //tu:            
          Bpr.OkFaF();
        }
        else {
          Bpr.ShortFaF();
        }
      }
      catch (Exception ex) { ex.Pop(); }
    }
    void filterTxns(string strFilter, string txCatgoryId)
    {
      tbkFlt.Text = strFilter;
      tbkAmt.Text = "";

      _core.ClearAddRangeAuto(_db.TxCoreV2.Local.Where(r => (
        (!string.IsNullOrEmpty(r.TxDetail) && r.TxDetail.ToLower().Contains(strFilter.ToLower())) ||
        (!string.IsNullOrEmpty(r.MemoPP) && r.MemoPP.ToLower().Contains(strFilter.ToLower())) ||
        (!string.IsNullOrEmpty(r.Notes) && r.Notes.ToLower().Contains(strFilter.ToLower())))
        &&
        (_cutOffYr == null ? r.TxDate >= _yrStart2004 : r.TxDate.Year >= _cutOffYr)
        &&
        (
          string.IsNullOrEmpty(txCatgoryId) ||
          (
            (
              (!string.IsNullOrEmpty(strFilter) ||
               r.TxDate.Year >= _trgTaxYr && string.Compare(r.TxCategoryIdTxt, txCatgoryId, true) == 0)
              ) ||
            (r.TxDate.Year < _trgTaxYr)
          )
        )
      ).OrderByDescending(r => r.TxDate));
    }
    void filterTxns(string strFilter, string txCatgoryId, decimal amt, decimal rng)
    {
      tbkFlt.Text = strFilter;
      tbkAmt.Text = $"{amt:N0}";

      _core.ClearAddRangeAuto(_db.TxCoreV2.Local.Where(r => (
        amt - rng <= (chkIsAbs.IsChecked == true ? Math.Abs(r.TxAmount) : r.TxAmount) && (chkIsAbs.IsChecked == true ? Math.Abs(r.TxAmount) : r.TxAmount) <= amt + rng
        ) && (
        (!string.IsNullOrEmpty(r.TxDetail) && r.TxDetail.ToLower().Contains(strFilter.ToLower())) ||
        (!string.IsNullOrEmpty(r.MemoPP) && r.MemoPP.ToLower().Contains(strFilter.ToLower())) ||
        (!string.IsNullOrEmpty(r.Notes) && r.Notes.ToLower().Contains(strFilter.ToLower())))
        &&
        (_cutOffYr == null ? r.TxDate >= _yrStart2004 : r.TxDate.Year >= _cutOffYr)
        &&
        (string.IsNullOrEmpty(txCatgoryId) || string.Compare(r.TxCategoryIdTxt, txCatgoryId, true) == 0)).OrderByDescending(r => r.TxDate));
    }
    void filterCategoryByIdList(IEnumerable<int> catIds) => updateCtgrList(_db.TxCategories.Local.Where(r => catIds.Contains(r.Id)));
    void filterCategoryByTxtMatch(string namePart) => updateCtgrList(_db.TxCategories.Local.Where(r => r.Name.ToLower().Contains(namePart) || r.IdTxt.ToLower().Contains(namePart)));

    void updateCtgrList(IEnumerable<TxCategory> enu)
    {
      _catg.ClearAddRangeAuto(enu);
      _txnCtg.Source = _catg;

      if (_catg.Count() == 0)
      {
        _catg.ClearAddRangeAuto(_db.TxCategories.Local.OrderBy(r => r.ExpenseGroup.Name).ThenBy(r => r.TL_Number));
      }

      choiceAbove.IsEnabled = choiceBelow.IsEnabled = btAssign.IsEnabled = btAssig2.IsEnabled = false;
      _choiceAbove = _choiceBelow = "";

      if (_catg.Count() == 1)
      {
        btAssign.IsEnabled = btAssig2.IsEnabled = true;
        _txnCtg.View.MoveCurrentToFirst();
      }
      else if (_catg.Count() == 2)
      {
        _choiceAbove = _catg.First().IdTxt;
        _choiceBelow = _catg.Last().IdTxt;
        btAssign.IsEnabled = btAssig2.IsEnabled = txCategoryListBox.SelectedItems.Count == 1;// _choiceAbove == _choiceBelow;
      }

      choiceAbove.Content = $"_1 {_choiceAbove}"; choiceAbove.IsEnabled = _choiceAbove.Length > 0;
      choiceBelow.Content = $"_7 {_choiceBelow}"; choiceBelow.IsEnabled = _choiceBelow.Length > 0;

      updateTitle();
    }

    async Task<bool> isStillTyping(TextBox textbox) { var tx = textbox.Text.ToLower(); await Task.Delay(750); return tx != textbox.Text.ToLower(); }
    void updateTitle() => Title = $"Ctgry: {_loadedCatgry}      {_core.Count()} rows    sum: {_core.Sum(r => r.TxAmount):C}    selects: {_selectTtl:C}       ";

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
    void onReLoad2(object s, RoutedEventArgs e) => ImportToDB.DoAll();

    async void onTextChangedFuz(object s, TextChangedEventArgs e) { if (!(await isStillTyping((TextBox)s))) await filterStart(tbxSearch.Text); }
    async void onTextChangedCtg(object s, TextChangedEventArgs e) { if (!(await isStillTyping((TextBox)s))) filterCategoryByTxtMatch(((TextBox)s).Text); }

    void onClear1(object s = null, RoutedEventArgs e = null) => tbkFlt.Text = tbxSearch.Text = "";
    void onClear2(object s = null, RoutedEventArgs e = null) => tSrch2.Text = "";

    void dgTxCtgr_SelectionChanged(object s, SelectionChangedEventArgs e) => btAssign.IsEnabled = btAssig2.IsEnabled = e.AddedItems.Count == 1;
    void dgTxCore_SelectionChanged(object s, SelectionChangedEventArgs e)
    {
      try
      {
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
            var trg = lv.Where(r => r.TxCategory.IdTxt != "UnKn")?.FirstOrDefault();
            _choiceAbove = _choiceBelow = trg?.TxCategory.IdTxt ?? "";
            btAssign.IsEnabled = btAssig2.IsEnabled = txCategoryListBox.SelectedItems.Count == 1;
          }
        }

        choiceAbove.Content = $"_1 {_choiceAbove}"; choiceAbove.IsEnabled = _choiceAbove.Length > 0;
        choiceBelow.Content = $"_7 {_choiceBelow}"; choiceBelow.IsEnabled = _choiceBelow.Length > 0;

        updateTitle();
      }
      catch (Exception ex) { ex.Pop(); }
    }

    void /**/  onTxCatgry_Checked(object s, RoutedEventArgs e) { cbxTxCatgry.Focus(); onTxCatgry_Changed(s, null); }
    async void onTxCatgry_UnChckd(object s, RoutedEventArgs e)           /**/ { _txCatgry = null;                                 /**/ await filterStart(tbxSearch.Text); }
    async void onTxCatgry_Changed(object s, SelectionChangedEventArgs e) /**/ { _txCatgry = (string)cbxTxCatgry.SelectedValue;    /**/ await filterStart(tbxSearch.Text); }

    void /**/  onSingleYr_Checked(object s, RoutedEventArgs e)           /**/ { cbxSingleYr?.Focus(); onSingleYr_Changed(s, null); }
    async void onSingleYr_UnChckd(object s, RoutedEventArgs e)           /**/ { _cutOffYr = null;                                 /**/ await filterStart(tbxSearch.Text); }
    async void onSingleYr_Changed(object s, SelectionChangedEventArgs e) /**/ { _cutOffYr = (int)cbxSingleYr.SelectedValue;       /**/ await filterStart(tbxSearch.Text); }

    void OnLbxSelectionChanged(object s, SelectionChangedEventArgs e) { if (e.AddedItems.Count < 1) return; updateTitle(); Title += string.Format(" - {0}", ((TxCategory)(txCategoryListBox.SelectedItems[0])).Name); }
    async void onInfer_Checked(object s, RoutedEventArgs e) => await filterStart(tbkFlt.Text);
    void dgTxCtgr_MouseDblClick(object s, System.Windows.Input.MouseButtonEventArgs e) { if (btAssign.IsEnabled) onAssign0(s, null); }
    void dgTxCore_MouseDblClick(object s, System.Windows.Input.MouseButtonEventArgs e)
    {
      if (e.OriginalSource is System.Windows.Documents.Run run)
        Clipboard.SetText(run.Text);
      else
        Clipboard.SetText(((TxCoreV2)((System.Windows.Controls.Primitives.Selector)s).SelectedItem).TxDetail);

      App.Synth.SpeakAsync("Copied!");
    }

    async void onManualTxnAdd(object s, RoutedEventArgs e) { new ManualTxnEntry(_db).ShowDialog(); await reLoadTxCore(); }
    async void onAssign0(object s, RoutedEventArgs e) { if (txCategoryListBox.SelectedItems.Count == 1) await assign(((TxCategory)(txCategoryListBox.SelectedItems[0])).IdTxt); }
    async void onAssign1(object s, RoutedEventArgs e) => await assign(_choiceAbove);
    async void onAssign2(object s, RoutedEventArgs e) => await assign(_choiceBelow);
    async Task assign(string IdTxt)
    {
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
        onClear1();
        onClear2();
      }

      tbxSearch.Focus();
      //Bpr.OkFaF();
    }
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
