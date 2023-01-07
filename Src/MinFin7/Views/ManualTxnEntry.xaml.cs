using System.ComponentModel;
using System.Globalization;
using EF.DbHelper.Lib;
using MinFin7;
using WpfUserControlLib.Extensions;

namespace MF.TxCategoryAssigner.Views;

public partial class ManualTxnEntry : WindowBase
{
  readonly FinDemoContext _db;
  readonly bool _toDispose;
  readonly DateTime _now = DateTime.Now;

  public ManualTxnEntry(bool showBackToMenuBtn) : this(db: null) => btnBackToMenu.Visibility = showBackToMenuBtn ? Visibility.Visible : Visibility.Collapsed;
  public ManualTxnEntry(FinDemoContext db) : this() => _db = (_toDispose = db == null) ? new() : db;

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
      await App.Synth.SpeakExpressAsync("Loading...!");

      await _db.TxCoreV2s.Where(r => r.TxDate.Year >= 2021).OrderByDescending(r => r.Id).Take(2).LoadAsync();
      await _db.TxCategories.LoadAsync();
      await _db.TxMoneySrcs.LoadAsync();

      ((CollectionViewSource)FindResource("txCoreV2ViewSource")).Source = _db.TxCoreV2s.Local;//.OrderBy(r => r.Id);
      ((CollectionViewSource)FindResource("txMoneySrcViewSource")).Source = _db.TxMoneySrcs.Local;
      ((CollectionViewSource)FindResource("txCategoryViewSource")).Source = _db.TxCategories.Local.OrderBy(r => r.Name);

      cbSrc.ItemsSource = _db.TxMoneySrcs.Local;

      _ = btnAddNewRcrd.Focus();

      await App.Synth.SpeakExpressAsync("Done!");
    }
    catch (Exception ex) { ex.Pop(); }
  }
  async void onSave(object sender = null, RoutedEventArgs e = null) => tbkTitle.Text = (await _db.TrySaveReportAsync()).report;
  void onAddingNewItem(object sender, System.Windows.Controls.AddingNewItemEventArgs e) => e.NewItem = createNewTxn(); //tu: pre-fill new record with valid values on the fly.
  void onMenu(object s, RoutedEventArgs e) { Hide(); _ = new Views.MainAppDispatcher().ShowDialog(); }
  void cbSrc_SelectionChanged(object s, System.Windows.Controls.SelectionChangedEventArgs e) => btnTD.IsEnabled = int.TryParse(tbYear.Text, out var yr) && yr > 2000 && cbSrc.SelectedValue as int? > 0;

  TxCoreV2 createNewTxn() => new()
  {
    CreatedAt = _now,
    FitId = $"{DateTime.Now:yyMMdd-HHmmss.fff}-_ManualAdd",
    TxDate = _now.Date,
    TxCategoryIdTxt = "UnKn",
    MemoPp = "manual entry",
    TxDetail = "..."
  };

  void onAddTxn(object s, RoutedEventArgs e)
  {
    _db.TxCoreV2s.Local.Add(createNewTxn());
    _ = ((CollectionViewSource)FindResource("txCoreV2ViewSource")).View.MoveCurrentToLast();
    _ = tbxMemo.Focus();
  }
  void onReadTD(object s, RoutedEventArgs e)
  {
    try
    {
      if (!Clipboard.ContainsText())
      {
        _ = MessageBox.Show("Clipboard is empty");
        return;
      }

      var cb = Clipboard.GetText();

      var i = 0;
      foreach (var line in cb.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
      {
        var words = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        if (words.Length < 5)
        {
          _ = MessageBox.Show(line, "Invalid entry");
          continue;
        }

        var txDateStr = $"{tbYear.Text} {words[0]} {words[1]}";
        if (!DateTime.TryParseExact(txDateStr, "yyyy MMM d", CultureInfo.InvariantCulture, DateTimeStyles.None, out var txDate))
        {
          _ = MessageBox.Show(txDateStr, "Invalid Tx Date");
          continue;
        }

        if (!decimal.TryParse(words.Last(), NumberStyles.Currency, null, out var txAmount))
        {
          _ = MessageBox.Show(words.Last(), "Invalid Tx Amount");
          continue;
        }

        if (_db.TxCoreV2s.Local.FirstOrDefault(r =>
          r.TxDate == txDate &&
          r.TxAmount == txAmount &&
          r.TxDetail == line[12..] &&
          r.Notes == line
        ) != null)
        {
          _ = MessageBox.Show($"The record already added:\n\n{line}", "Duplicate Ignored");
          continue;
        }

        _db.TxCoreV2s.Local.Add(new TxCoreV2
        {
          CreatedAt = _now,
          FitId = $"{DateTime.Now:yyMMdd-HHmmss.fff}-{++i}_ManualAdd",
          TxDate = txDate,
          TxAmount = txAmount,
          TxMoneySrcId = (int)cbSrc.SelectedValue,
          TxCategoryIdTxt = "UnKn",
          TxDetail = line[12..],
          MemoPp = "manual entry",
          Notes = line
        });
        _ = ((CollectionViewSource)FindResource("txCoreV2ViewSource")).View.MoveCurrentToLast();
        _ = tbxMemo.Focus();
      }
    }
    catch (Exception ex) { ex.Pop(); }
  }
}
/// EXEC sp_changedbowner 'sa' //tu: dbo is missing fo db diagramming
/// 