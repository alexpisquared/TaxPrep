namespace MF.TxCategoryAssigner.Views;
public partial class MainAppDispatcher : WindowBase
{
  Db.FinDemo7.Models.FinDemoContext? _dba;
  SpeechSynth ? _sth;
  ILogger ? _lgr;
  IBpr ? _bpr;

  public MainAppDispatcher() // : this(SeriLogHelper.InitLoggerFactory(folder: FSHelper.GetCreateSafeLogFolderAndFile(@$"C:\Temp\Logs\{Assembly.GetExecutingAssembly().GetName().Name![..5]}.{VersionHelper.Env()}.{Environment.UserName[..3]}..log"), levels: "+Info").CreateLogger(), new Bpr())
  {
    InitializeComponent();
  }
  public MainAppDispatcher(ILogger lgr, IBpr bpr, SpeechSynth sth, FinDemoContext dba)
  {
    InitializeComponent();

    _lgr = lgr;
    _bpr = bpr;
    _sth = sth;
    _dba = dba;
    KeyUp += (s, e) =>
    {
      //switch (e.Key)
      //{
      //  case Key.Escape: Close(); /*App.Current.Shutdown();*/ break;
      //  case Key.Up:   /**/ Settings.Default.LastBtnNo = Settings.Default.LastBtnNo > 0                /**/ ? --Settings.Default.LastBtnNo : 0;                /**/ SetDefault(Settings.Default.LastBtnNo); break;
      //  case Key.Down: /**/ Settings.Default.LastBtnNo = Settings.Default.LastBtnNo < _zeroBasedBtnCnt /**/ ? ++Settings.Default.LastBtnNo : _zeroBasedBtnCnt; /**/ SetDefault(Settings.Default.LastBtnNo); break;
      //  default: break;
      //}
    }; //tu:

    //SetDefault(Settings.Default.LastBtnNo);
    tbver.Text = $"Old pure MDI ver - {VersionHelper.CurVerStrYMd}";

    KeepOpenReason = ""; // nothing ot save/worry about at this stage for this window.
  }

  void SetDefault(int cb)
  {
    ArgumentNullException.ThrowIfNull(_lgr, nameof(_lgr));
    ArgumentNullException.ThrowIfNull(_bpr, nameof(_bpr));
    ArgumentNullException.ThrowIfNull(_sth, nameof(_sth));
    ArgumentNullException.ThrowIfNull(_dba, nameof(_dba));

    _bpr.Click();
    b1.IsDefault = b2.IsDefault = b3.IsDefault = b4.IsDefault = b5.IsDefault = b6.IsDefault = b7.IsDefault = b8.IsDefault = false;
    ((Button)FindName($"b{cb + 1}")).IsDefault = true;
    //Settings.Default.LastBtnNo = cb;
    //Settings.Default.Save();
  }

  async void onX(object s, RoutedEventArgs e)
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
      //case "b5": setDefault(4); MinFin7MdiLib.Report.WinForm.Program.ShowBoth(); break;
      //case "b6": setDefault(5); MinFin7MdiLib.Report.WinForm.Program.Show_Alx(); break;
      //case "b7": setDefault(6); MinFin7MdiLib.Report.WinForm.Program.Show_Mei(); break;
      case "b8": SetDefault(7); new ReviewWindow(_lgr, _bpr, "Mei", _dba).Show(); break;
      //case "b9": setDefault(8); new MinFin7MdiLib.DataSet.TxAdd().Show(); break;
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
