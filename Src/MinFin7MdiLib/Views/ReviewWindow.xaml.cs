namespace MinFin7MdiLib.Views;
public partial class ReviewWindow : WindowBase
{
  readonly FinDemoContext _dba;
  readonly ILogger _lgr;
  readonly IBpr _bpr;
  string _own;
  bool _busy;

  public ReviewWindow(ILogger lgr, IBpr bpr, string owner, FinDemoContext dba)
  {
    InitializeComponent();
    _lgr = lgr;
    _bpr = bpr;
    _dba = dba;
    _own = owner;
    KeepOpenReason = ""; // this ir RO window.
  }

  async void OnLoaded(object sender, RoutedEventArgs e)
  {
    try
    {
      _busy = true;
      ctrlPanel.Visibility = dgTxVs.Visibility = Visibility.Hidden;
      _bpr.Start(12);
      await Task.Yield();                       // it really shows window on .Net 4.8 !!! (2022-Jan-30)
      //await _dba.VwTxCores.LoadAsync();         // TxCoreV2 would show the MoneySrc.Name in Binding
      //await _dba.VwExpHistVsLasts.LoadAsync();

      /*
      dgTxVs.ItemsSource = _dba.VwExpHistVsLasts.Local.OrderBy(r => r.Name).ThenBy(r => r.TaxLiq);
      This error happens because the entity type VwExpHistVsLast does not have a primary key defined, and Entity Framework Core requires a primary key for tracking entities.
This might be happening because:
1.	The VwExpHistVsLast class is marked as [Keyless], indicating it does not have a primary key.
2.	The Local property of a DbSet relies on the primary key to track changes, which is why the exception is thrown when trying to access _dba.VwExpHistVsLasts.Local.
To fix this issue, you can:
1.	Define a primary key for the VwExpHistVsLast entity if possible.
2.	If the entity is inherently keyless, avoid using the Local property. Instead, you can load the data directly into a list or another collection.
      */
      var expHistVsLasts = await _dba.VwExpHistVsLasts.ToListAsync();
      dgTxVs.ItemsSource = expHistVsLasts.OrderBy(r => r.Name).ThenBy(r => r.TaxLiq);

      dbHist.ItemsSource = null;
      _ = dgTxVs.Focus();
      _bpr.Finish(12);
    }
    catch (Exception ex) { ex.Pop(_lgr); }
    finally { _busy = false; ctrlPanel.Visibility = dgTxVs.Visibility = Visibility.Visible; }
  }
  async void OnDataGridSelectionChanged(object s, SelectionChangedEventArgs e)
  {
    try
    {
      if (e.AddedItems.Count > 0)
      {
        _bpr.Tick();
        //dbHist.ItemsSource = _dba.VwTxCores.Local.Where(r => string.Compare(r.TxCategoryIdTxt, ((VwExpHistVsLast)((object[])e.AddedItems)[0]).IdTxt, true) == 0).OrderByDescending(r => r.TxDate);
        var ItemsSource = await _dba.VwTxCores
          .Where(r => r.TxCategoryIdTxt == ((VwExpHistVsLast)((object[])e.AddedItems)[0]).IdTxt)
          .ToListAsync();
        dbHist.ItemsSource = ItemsSource.OrderByDescending(r => r.TxDate);
        dbHist.SelectedItem = null;
      }
      else
        _bpr.No();
    }
    catch (Exception ex) { ex.Pop(_lgr); }
  }
  void OnUserChecked(object s, RoutedEventArgs e)
  {
    switch (_own = ((RadioButton)s).Name)
    {
      case "Alx": _dba.VwExpHistVs2018Alxes.Load(); dgTxVs.ItemsSource = _dba.VwExpHistVs2018Alxes.Local.OrderBy(r => r.Name).ThenBy(r => r.TaxLiq); break;
      case "Mei": _dba.VwExpHistVs2018Meis.Load(); dgTxVs.ItemsSource = _dba.VwExpHistVs2018Meis.Local.OrderBy(r => r.Name).ThenBy(r => r.TaxLiq); break;
      case "Ndn": _dba.VwExpHistVs2018Ndns.Load(); dgTxVs.ItemsSource = _dba.VwExpHistVs2018Ndns.Local.OrderBy(r => r.Name).ThenBy(r => r.TaxLiq); break;
      case "Zoe": _dba.VwExpHistVs2018Zoes.Load(); dgTxVs.ItemsSource = _dba.VwExpHistVs2018Zoes.Local.OrderBy(r => r.Name).ThenBy(r => r.TaxLiq); break;
      default: _dba.VwExpHistVsLasts.Load(); dgTxVs.ItemsSource = _dba.VwExpHistVsLasts.Local.OrderBy(r => r.Name).ThenBy(r => r.TaxLiq); break;
    }

    CollectionViewSource.GetDefaultView(dgTxVs.ItemsSource).Refresh(); //tu: refresh bound DataGrid
  }
}
