namespace MinFin7.Views;
public partial class ReviewWindow : WindowBase
{
  readonly FinDemoContext _db = new();
  readonly ILogger _lgr;
  readonly IBpr _bpr;
  string _owner;

  public ReviewWindow(ILogger lgr, IBpr bpr, string owner)
  {
    InitializeComponent();
    _lgr = lgr;
    _bpr = bpr;
    _owner = owner;
  }

  async void OnLoaded(object sender, RoutedEventArgs e)
  {
    try
    {
      _bpr.Start();
      await Task.Yield();                 // it really shows window on .Net 4.8 !!! (2022-Jan-30)
      await _db.VwTxCores.LoadAsync();            // TxCoreV2 would show the MoneySrc.Name in Binding
      await _db.VwExpHistVsLasts.LoadAsync();
      dgTxVs.ItemsSource = _db.VwExpHistVsLasts.Local.OrderBy(r => r.Name).ThenBy(r => r.TaxLiq);
      dbHist.ItemsSource = null;
      dgTxVs.Focus();
      _bpr.Finish();
    }
    catch (Exception ex) { ex.Pop(_lgr); }
  }
  void dgCore_SelnChgd(object s, SelectionChangedEventArgs e)
  {
    if (e.AddedItems.Count > 0)
    {
      _bpr.Tick();
      dbHist.ItemsSource = _db.VwTxCores.Local.Where(r => string.Compare(r.TxCategoryIdTxt, ((VwExpHistVsLast)((object[])e.AddedItems)[0]).IdTxt, true) == 0).OrderByDescending(r => r.TxDate);
      dbHist.SelectedItem = null;
    }
    else
      _bpr.No();
  }
  void onUserChecked(object s, RoutedEventArgs e)
  {
    switch (_owner = ((RadioButton)s).Name)
    {
      case "Alx": _db.VwExpHistVs2018Alxes.Load(); dgTxVs.ItemsSource = _db.VwExpHistVs2018Alxes.Local.OrderBy(r => r.Name).ThenBy(r => r.TaxLiq); break;
      case "Mei": _db.VwExpHistVs2018Meis.Load(); dgTxVs.ItemsSource = _db.VwExpHistVs2018Meis.Local.OrderBy(r => r.Name).ThenBy(r => r.TaxLiq); break;
      case "Ndn": _db.VwExpHistVs2018Ndns.Load(); dgTxVs.ItemsSource = _db.VwExpHistVs2018Ndns.Local.OrderBy(r => r.Name).ThenBy(r => r.TaxLiq); break;
      case "Zoe": _db.VwExpHistVs2018Zoes.Load(); dgTxVs.ItemsSource = _db.VwExpHistVs2018Zoes.Local.OrderBy(r => r.Name).ThenBy(r => r.TaxLiq); break;
      default: _db.VwExpHistVsLasts.Load(); dgTxVs.ItemsSource = _db.VwExpHistVsLasts.Local.OrderBy(r => r.Name).ThenBy(r => r.TaxLiq); break;
    }

    CollectionViewSource.GetDefaultView(dgTxVs.ItemsSource).Refresh(); //tu: refresh bound datagrid
  }
}
