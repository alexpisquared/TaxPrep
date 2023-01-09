namespace MinFin7.Views;
public partial class ReviewWindow : WindowBase
{
  readonly FinDemoContext _db = new();
  readonly DateTime _yrStart2004 = new(2004, 1, 1), _now = DateTime.Now;
  readonly ObservableCollection<TxCoreV2> _core = new();
  readonly ObservableCollection<TxCategory> _catg = new();
  readonly CollectionViewSource _txCategoryCmBxVwSrc, _txCategoryDGrdVwSrc, _txCoreV2_Root_VwSrc;
  readonly int _trgTaxYr = DateTime.Today.Year - 1;
  readonly ILogger<TxCategoryAssignerVw> _lgr;
  readonly Bpr _bpr;
  string? _txCatgry, _loadedCatgry = "?", _choiceAbove, _choiceBelow;
  decimal _selectTtl = 0;
  bool _loaded = false;
  int? _cutOffYr = null;
  string _owner;

  public ReviewWindow(ILogger<TxCategoryAssignerVw> lgr, Bpr bpr, string owner)
  {
    InitializeComponent();
    //_txCategoryCmBxVwSrc = (CollectionViewSource)FindResource("txCategoryCmBxVwSrc");
    //_txCategoryDGrdVwSrc = (CollectionViewSource)FindResource("txCategoryDGrdVwSrc");
    //_txCoreV2_Root_VwSrc = (CollectionViewSource)FindResource("txCoreV2_Root_VwSrc");
    _lgr = lgr;
    _bpr = bpr;
  }

  async void OnLoaded(object sender, RoutedEventArgs e)
  {
    try
    {
      _bpr.Start();
      await Task.Yield(); // it really shows window on .Net 4.8 !!! (2022-Jan-30)
      await _db.VwTxCores.LoadAsync()/*.ConfigureAwait(false)*/;            // TxCoreV2 would show the MoneySrc.Name in Binding
      await _db.VwExpHistVsLasts.LoadAsync()/*.ConfigureAwait(false)*/;
      dgTxVs.ItemsSource = _db.VwExpHistVsLasts.Local.OrderBy(r => r.Name).ThenBy(r => r.TaxLiq);
      dbHist.ItemsSource = null;
      dgTxVs.Focus();
      _bpr.Finish();
    }
    catch (Exception ex) { ex.Pop(); }
  }
  void dgCore_SelnChgd(object s, SelectionChangedEventArgs e)
  {
    if (e.AddedItems.Count > 0)
    {
      _bpr.Tick();
      dbHist.ItemsSource = _db.VwTxCores.Local.Where(r => string.Compare(r.TxCategoryIdTxt, ((VwExpHistVsLast)((object[])e.AddedItems)[0]).IdTxt, true) == 0).OrderByDescending(r => r.TxDate);
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
