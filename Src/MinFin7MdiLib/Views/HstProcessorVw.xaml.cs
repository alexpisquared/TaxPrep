namespace MF.TxCategoryAssigner.Views
{
  public partial class HstProcessorVw : WindowBase
  {
    readonly FinDemoContext _db;
    //readonly DateTime _now = DateTime.Now;
    DateTime _qtrStart = new DateTime(2017, 1, 1);
    DateTime _qtr__End = new DateTime(2017, 4, 1);

    public HstProcessorVw() => InitializeComponent(); // KeyDown += (s, e) => { switch (e.Key) { case Key.Escape: Close(); break; } };
    async void onLoaded(object s, RoutedEventArgs e)
    {
      await reLoadTxCore();
      reload(0);

      //tSrch.Focus();
      //_bpr.BeepOk();
    }

    async Task reLoadTxCore()
    {
      try
      {
        //await dispatchIfNecessary(async () => 
        await _db.TxCoreV2s.Where(r => r.TxDate >= _qtrStart && r.TxDate < _qtr__End && (r.TxMoneySrcId == 3) && (r.TxAmount < 0) && (!r.TxCategoryIdTxt.Equals("BankFee", StringComparison.OrdinalIgnoreCase))).LoadAsync()
      //)
      ;

        ((CollectionViewSource)(FindResource("txCoreV2ViewSource"))).Source = _db.TxCoreV2s.Local.OrderBy(r => r.TxDate);

        //if (chkTxCatgry.IsChecked == true)
        //{
        //  await Task.Run(() => _dba.TxCoreV2s.Where(r => r.TxDate >= yrStart2004 && (string.Compare(r.TxCategoryIdTxt, _txCatgry, true) == 0)).Load());
        //  _loadedCatgry = _txCatgry;
        //}
        //else
        //{
        //  await Task.Run(() => _dba.TxCoreV2s.Where(r => r.TxDate >= yrStart2004).Load());
        //  _loadedCatgry = _txCatgry = null;
        //}
      }
      catch (Exception ex) { ex.Log(); throw; }
    }

    void onPrevQtr(object s, RoutedEventArgs e) => reload(-3);
    void onNextQtr(object s, RoutedEventArgs e) => reload(+3);
    void onDoToday(object s, RoutedEventArgs e) { }

    void reload(int m3)
    {
      _qtrStart = _qtrStart.AddMonths(m3);
      _qtr__End = _qtr__End.AddMonths(m3);

      tA.Text = $"{_qtrStart:yyyy-MM-dd}";
      tB.Text = $"{_qtr__End:yyyy-MM-dd}";

      //await reLoadTxCore();
      _db.TxCoreV2s.Where(r => r.TxDate >= _qtrStart && r.TxDate < _qtr__End && (r.TxMoneySrcId == 3) && (r.TxAmount < 0) && (!r.TxCategoryIdTxt.Equals("BankFee", StringComparison.OrdinalIgnoreCase))).Load();
      ((CollectionViewSource)(FindResource("txCoreV2ViewSource"))).Source = _db.TxCoreV2s.Local.Where(r => r.TxDate >= _qtrStart && r.TxDate < _qtr__End && (r.TxMoneySrcId == 3) && (r.TxAmount < 0) && (!r.TxCategoryIdTxt.Equals("BankFee", StringComparison.OrdinalIgnoreCase))).OrderBy(r => r.TxDate);
      var ttl = _db.TxCoreV2s.Local.Where(r => r.TxDate >= _qtrStart && r.TxDate < _qtr__End && (r.TxMoneySrcId == 3) && (r.TxAmount < 0) && (!r.TxCategoryIdTxt.Equals("BankFee", StringComparison.OrdinalIgnoreCase))).Sum(r => r.TxAmount);
      tS.Text = $"{(ttl / 1.13m),12:N2}  {(ttl),12:N2}  {((ttl / 1.13m * .088m) + (_qtrStart.Month == 1 ? 300m : 0m)),12:N2}  ";
    }
  }
}
