namespace MinFin7MdiLib.Views;
public partial class MdiMenuView : UserControl
{
  Db.FinDemo7.Models.FinDemoContext? _dba;
  SpeechSynth? _sth;
  ILogger? _lgr;
  IBpr? _bpr;

  public MdiMenuView() { InitializeComponent(); }
  void OnLoaded(object sender, RoutedEventArgs e)
  {
    _lgr = ((BaseDbVM)DataContext).Lgr;
    _bpr = ((BaseDbVM)DataContext).Bpr;
    _sth = ((BaseDbVM)DataContext).Sth;
    _dba = ((BaseDbVM)DataContext).Dba;
  }

  void SetDefault(int cb) { b1.IsDefault = b2.IsDefault = b3.IsDefault = b4.IsDefault = b5.IsDefault = b6.IsDefault = b7.IsDefault = b8.IsDefault = false; ((Button)FindName($"b{cb + 1}")).IsDefault = true; }
  async void OnX(object s, RoutedEventArgs e)
  {
    ArgumentNullException.ThrowIfNull(_lgr, nameof(_lgr));
    ArgumentNullException.ThrowIfNull(_bpr, nameof(_bpr));
    ArgumentNullException.ThrowIfNull(_sth, nameof(_sth));
    ArgumentNullException.ThrowIfNull(_dba, nameof(_dba));

    ((Button)s).IsEnabled = false;
    switch (((Button)s).Name)
    {
      //case "b1": setDefault(0); new DbLoaderReportWindow(MSMoneyDbLoader.App.GetCmndLineArgsInclClickOnce()).ShowDialog(); break;
      //case "b2": setDefault(1); new HistoricalChartSet.MainHistChart().Show(); break;
      case "b3": SetDefault(2); new TxCategoryAssignerVw(_lgr, _bpr, _sth, _dba).Show(); break;
      case "b4": SetDefault(3); new ManualTxnEntry(_lgr, _bpr, _sth, false, _dba).Show(); break;
      case "b5": SetDefault(4); MinFin.Report.WinForm7.Program.ShowBoth(); break;
      case "b6": SetDefault(5); new PrintableReport(_lgr, _bpr, "Alx", _dba).Show(); break;
      case "b7": SetDefault(6); MinFin.Report.WinForm7.Program.Show_Mei(); break;
      case "b8": SetDefault(7); new ReviewWindow(_lgr, _bpr, "Mei", _dba).Show(); break;
      //case "b9": setDefault(8); new MinFin7MdiLib.DataSet.TxAdd().Show(); break;
      default: _bpr?.No(); break;
    }

    await Task.Delay(500);
    ((Button)s).IsEnabled = true;
  }
  void OnNavigate(object s, System.Windows.Navigation.RequestNavigateEventArgs e)
  {
    var dir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), e.Uri.OriginalString); // ~C:\Users\alexp\OneDrive\Documents\0\MF\DnLds
    try { _ = Process.Start(new ProcessStartInfo(dir)); e.Handled = true; } catch (Exception ex) { _ = ex.Log(dir); }
  }
}