namespace MF.TxCategoryAssigner.Views;
public partial class MainAppDispatcher : WindowBase
{
  const int _zeroBasedBtnCnt = 8;
  readonly ILogger<TxCategoryAssignerVw> _lgr;
  readonly Bpr _bpr;

  public MainAppDispatcher(ILogger<TxCategoryAssignerVw> lgr, Bpr bpr)
  {
    InitializeComponent();

    _lgr = lgr;
    _bpr = bpr;

    KeyUp += (s, e) =>
    {
      switch (e.Key)
      {
        case Key.Escape: Close(); App.Current.Shutdown(); break;
        case Key.Up:   /**/ Settings.Default.LastBtnNo = Settings.Default.LastBtnNo > 0                /**/ ? --Settings.Default.LastBtnNo : 0;                /**/ SetDefault(Settings.Default.LastBtnNo); break;
        case Key.Down: /**/ Settings.Default.LastBtnNo = Settings.Default.LastBtnNo < _zeroBasedBtnCnt /**/ ? ++Settings.Default.LastBtnNo : _zeroBasedBtnCnt; /**/ SetDefault(Settings.Default.LastBtnNo); break;
        default: break;
      }
    }; //tu:

    SetDefault(Settings.Default.LastBtnNo);
    tbver.Text = VersionHelper.CurVerStrYMd;
  }

  void SetDefault(int cb)
  {
    _bpr.Click();
    b1.IsDefault = b2.IsDefault = b3.IsDefault = b4.IsDefault = b5.IsDefault = b6.IsDefault = b7.IsDefault = b8.IsDefault = false;
    ((Button)FindName($"b{cb + 1}")).IsDefault = true;
    Settings.Default.LastBtnNo = cb;
    Settings.Default.Save();
  }

  async void onX(object s, RoutedEventArgs e)
  {
    ((Button)s).IsEnabled = false;
    switch (((Button)s).Name)
    {
      //case "b1": setDefault(0); new DbLoaderReportWindow(MSMoneyDbLoader.App.GetCmndLineArgsInclClickOnce()).ShowDialog(); break;
      //case "b2": setDefault(1); new HistoricalChartSet.MainHistChart().Show(); break;
      case "b3": SetDefault(2); new TxCategoryAssignerVw(_lgr, _bpr).Show(); break;
      case "b4": SetDefault(3); new ManualTxnEntry(_lgr, _bpr, false).Show(); break;
      //case "b5": setDefault(4); MinFin7.Report.WinForm.Program.ShowBoth(); break;
      //case "b6": setDefault(5); MinFin7.Report.WinForm.Program.Show_Alx(); break;
      //case "b7": setDefault(6); MinFin7.Report.WinForm.Program.Show_Mei(); break;
      case "b8": SetDefault(7); new ReviewWindow(_lgr, _bpr, "Mei").Show(); break;
      //case "b9": setDefault(8); new MinFin7.DataSet.TxAdd().Show(); break;
      default: _bpr.No(); break;
    }

    await Task.Delay(500);
    ((Button)s).IsEnabled = true;
  }

  void onNavigate(object s, System.Windows.Navigation.RequestNavigateEventArgs e)
  {
    var dir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), e.Uri.OriginalString); // ~C:\Users\alexp\OneDrive\Documents\0\MF\DnLds
    try { _ = Process.Start(new ProcessStartInfo(dir)); e.Handled = true; } catch (Exception ex) { _ = ex.Log(dir); }
  }

  void BClose_Click(object s, RoutedEventArgs e) => Close();
}
