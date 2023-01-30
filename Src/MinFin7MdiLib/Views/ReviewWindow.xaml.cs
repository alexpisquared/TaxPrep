namespace MinFin7MdiLib.Views;
public partial class ReviewWindow : WindowBase
{
  readonly Db.FinDemo7.Models.FinDemoContext _dba;
  readonly ILogger _lgr;
  readonly IBpr _bpr;
  string _onr;

  public ReviewWindow(ILogger lgr, IBpr bpr, string owner, FinDemoContext dba)
  {
    InitializeComponent();
    _lgr = lgr;
    _bpr = bpr;
    _dba = dba;
    _onr = owner;
    KeepOpenReason = ""; // this ir RO window.
  }

  async void OnLoaded(object sender, RoutedEventArgs e)
  {
    try
    {
      _bpr.Start();
      await Task.Yield();                 // it really shows window on .Net 4.8 !!! (2022-Jan-30)
      await _dba.VwTxCores.LoadAsync();            // TxCoreV2 would show the MoneySrc.Name in Binding
      await _dba.VwExpHistVsLasts.LoadAsync();
      dgTxVs.ItemsSource = _dba.VwExpHistVsLasts.Local.OrderBy(r => r.Name).ThenBy(r => r.TaxLiq);
      dbHist.ItemsSource = null;
      _ = dgTxVs.Focus();
      _bpr.Finish();
    }
    catch (Exception ex) { ex.Pop(_lgr); }
  }
  void dgCore_SelnChgd(object s, SelectionChangedEventArgs e)
  {
    if (e.AddedItems.Count > 0)
    {
      _bpr.Tick();
      dbHist.ItemsSource = _dba.VwTxCores.Local.Where(r => string.Compare(r.TxCategoryIdTxt, ((VwExpHistVsLast)((object[])e.AddedItems)[0]).IdTxt, true) == 0).OrderByDescending(r => r.TxDate);
      dbHist.SelectedItem = null;
    }
    else
      _bpr.No();
  }
  void onUserChecked(object s, RoutedEventArgs e)
  {
    switch (_onr = ((RadioButton)s).Name)
    {
      case "Alx": _dba.VwExpHistVs2018Alxes.Load(); dgTxVs.ItemsSource = _dba.VwExpHistVs2018Alxes.Local.OrderBy(r => r.Name).ThenBy(r => r.TaxLiq); break;
      case "Mei": _dba.VwExpHistVs2018Meis.Load(); dgTxVs.ItemsSource = _dba.VwExpHistVs2018Meis.Local.OrderBy(r => r.Name).ThenBy(r => r.TaxLiq); break;
      case "Ndn": _dba.VwExpHistVs2018Ndns.Load(); dgTxVs.ItemsSource = _dba.VwExpHistVs2018Ndns.Local.OrderBy(r => r.Name).ThenBy(r => r.TaxLiq); break;
      case "Zoe": _dba.VwExpHistVs2018Zoes.Load(); dgTxVs.ItemsSource = _dba.VwExpHistVs2018Zoes.Local.OrderBy(r => r.Name).ThenBy(r => r.TaxLiq); break;
      default: _dba.VwExpHistVsLasts.Load(); dgTxVs.ItemsSource = _dba.VwExpHistVsLasts.Local.OrderBy(r => r.Name).ThenBy(r => r.TaxLiq); break;
    }

    CollectionViewSource.GetDefaultView(dgTxVs.ItemsSource).Refresh(); //tu: refresh bound datagrid
  }
}
