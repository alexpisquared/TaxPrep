namespace MF.TxCategoryAssigner.Views;

public partial class ManualTxnEntry : WindowBase
{
  readonly FinDemoContext _db;
  readonly bool _toDispose;
  readonly DateTime _now = DateTime.Now;
  readonly ILogger<TxCategoryAssignerVw> _lgr;
  readonly Bpr _bpr;

  public ManualTxnEntry(ILogger<TxCategoryAssignerVw> lgr, Bpr bpr, bool showBackToMenuBtn, FinDemoContext? db = null)
  {
    InitializeComponent();

    _lgr = lgr;
    _bpr = bpr;
    _db = db is null ? new FinDemoContext() : db;
    _toDispose = db is null;

    btnBackToMenu.Visibility = showBackToMenuBtn ? Visibility.Visible : Visibility.Collapsed;
  }

  protected override void OnClosing(CancelEventArgs e)
  {
    if (_db.HasUnsavedChanges())
    {
      switch (MessageBox.Show($"Would you like to save the changes? \r\n\n{_db.GetDbChangesReport()}", "Unsaved changes detected", MessageBoxButton.YesNoCancel, MessageBoxImage.Question))
      {
        default:
        case MessageBoxResult.Cancel: e.Cancel = true; return;
        case MessageBoxResult.Yes: tbkTitle.Text = _db.TrySaveReportAsync().Result.report; break;
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
      App.Synth.SpeakExpressFAF("Loading...!");

      await _db.TxCoreV2s.Where(r => r.TxDate.Year >= 2021).OrderByDescending(r => r.Id).Take(2).LoadAsync();
      await _db.TxCategories.LoadAsync();
      await _db.TxMoneySrcs.LoadAsync();

      //2023.01.09 
      //Data binding directly to 'DbSet.Local' is not supported since it does not provide a stable ordering.
      //For WPF, bind to 'DbSet.Local.ToObservableCollection'.
      //For WinForms, bind to 'DbSet.Local.ToBindingList'.
      //For ASP.NET WebForms, bind to 'DbSet.ToList' or use Model Binding.
      //NotSupportedException at C:\g\TaxPrep\Src\MinFin7\Views\ManualTxnEntry.xaml.cs(62): onLoaded() 

      ((CollectionViewSource)FindResource("txCoreV2ViewSource")).Source = _db.TxCoreV2s.Local.ToObservableCollection();//.OrderBy(r => r.Id);
      ((CollectionViewSource)FindResource("txMoneySrcViewSource")).Source = _db.TxMoneySrcs.Local.ToObservableCollection();
      ((CollectionViewSource)FindResource("txCategoryViewSource")).Source = _db.TxCategories.Local.ToObservableCollection().OrderBy(r => r.Name);

      cbSrc.ItemsSource = _db.TxMoneySrcs.Local.ToObservableCollection();

      _ = btnAddNewRcrd.Focus();

      await App.Synth.SpeakExpressAsync("Done!");
    }
    catch (Exception ex) { ex.Pop(_lgr); }
  }
  async void onSave(object sender, RoutedEventArgs e) { _bpr.Click(); tbkTitle.Text = (await _db.TrySaveReportAsync()).report; }
  void onAddingNewItem(object sender, AddingNewItemEventArgs e) { _bpr.Click(); e.NewItem = createNewTxn(); }//tu: pre-fill new record with valid values on the fly.
  void onMenu(object s, RoutedEventArgs e) { _bpr.Click(); /*MessageBox.Show("N/A"); } //   {*/ Hide(); _ = new MainAppDispatcher(_lgr, _bpr).ShowDialog(); Show(); }
  void cbSrc_SelectionChanged(object s, SelectionChangedEventArgs e) { _bpr.Click(); btnTD.IsEnabled = int.TryParse(tbYear.Text, out var yr) && yr > 2000 && cbSrc.SelectedValue as int? > 0; }

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
    catch (Exception ex) { ex.Pop(_lgr); }
  }
}
/// EXEC sp_changedbowner 'sa' //tu: dbo is missing fo _db diagramming
/// 