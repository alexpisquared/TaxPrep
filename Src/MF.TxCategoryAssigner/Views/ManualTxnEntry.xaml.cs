using AAV.WPF.Ext;
using AsLink;
using Db.FinDemo.DbModel;
using System;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Speech.Synthesis;
using System.Windows;
using System.Windows.Data;

namespace MF.TxCategoryAssigner.Views
{
  public partial class ManualTxnEntry : AAV.WPF.Base.WindowBase
  {
    readonly A0DbContext _db;
    readonly bool _toDispose;

    public ManualTxnEntry(bool showBackToMenuBtn) : this(db: null) => btnBackToMenu.Visibility = showBackToMenuBtn ? Visibility.Visible : Visibility.Collapsed;
    public ManualTxnEntry(A0DbContext db) : this() => _db = (_toDispose = db == null) ? A0DbContext.Create() : db;

    ManualTxnEntry() => InitializeComponent();

    protected override void OnClosing(CancelEventArgs e)
    {
      if (_db.HasUnsavedChanges())
      {
        switch (MessageBox.Show($"Would you like to save the changes? \r\n\n{_db.GetDbChangesReport()}", "Unsaved changes detected", MessageBoxButton.YesNoCancel, MessageBoxImage.Question))
        {
          default:
          case MessageBoxResult.Cancel: e.Cancel = true; return;
          case MessageBoxResult.Yes: onSave(); break;
          case MessageBoxResult.No: break;
        }
      }

      if (_toDispose)
        _db.Dispose();

      base.OnClosing(e);
    }

    async void onLoaded(object s, RoutedEventArgs e)
    {
      try
      {
        App.Synth.Rate = +7; App.Synth.SelectVoiceByHints(VoiceGender.Female);
        App.Synth.SpeakAsync("Loading...!");

        await _db.TxCoreV2.Where(r => r.TxDate.Year >= 2018).OrderByDescending(r => r.Id).Take(5).LoadAsync();
        await _db.TxCategories.LoadAsync();
        await _db.TxMoneySrcs.LoadAsync();

        ((CollectionViewSource)(FindResource("txCoreV2ViewSource"))).Source = _db.TxCoreV2.Local;
        ((CollectionViewSource)(FindResource("txMoneySrcViewSource"))).Source = _db.TxMoneySrcs.Local;
        ((CollectionViewSource)(FindResource("txCategoryViewSource"))).Source = _db.TxCategories.Local.OrderBy(r => r.Name);

        btnAddNewRcrd.Focus();

        App.Synth.SpeakAsync("Done!");
      }
      catch (Exception ex) { ex.Pop(); }
    }
    void onSave(object sender = null, RoutedEventArgs e = null) => tbkTitle.Text = _db.TrySaveReport().report;
    void onAddTxn(object s, RoutedEventArgs e)
    {
      _db.TxCoreV2.Local.Add(createNewTxn());
      ((CollectionViewSource)(FindResource("txCoreV2ViewSource"))).View.MoveCurrentToLast();
      tbxMemo.Focus();
    }
    void onAddingNewItem(object sender, System.Windows.Controls.AddingNewItemEventArgs e) => e.NewItem = createNewTxn(); //tu: pre-fill new record with valid values on the fly.
    void onMenu(object s, RoutedEventArgs e) { Hide(); new Views.MainAppDispatcher().ShowDialog(); }

    TxCoreV2 createNewTxn()
    {
      var now = DateTime.Now;
      return new TxCoreV2
      {
        CreatedAt = now,
        FitId = $"{now:yyyyMMdd-HHmmss.fff}_ManualAdd",
        TxDate = now.Date,
        TxCategoryIdTxt = "UnKn",
        TxDetail = "..."
      };
    }
  }
}
/// EXEC sp_changedbowner 'sa' //tu: dbo is missing fo db diagramming
/// 