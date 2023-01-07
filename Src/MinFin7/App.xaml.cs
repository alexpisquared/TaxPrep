using MF.TxCategoryAssigner;

namespace MinFin7;
public partial class App : Application
{
  protected override void OnStartup(StartupEventArgs e)
  {
    base.OnStartup(e);
    Current.DispatcherUnhandledException += UnhandledExceptionHndlr.OnCurrentDispatcherUnhandledException;
    EventManager.RegisterClassHandler(typeof(TextBox), UIElement.GotFocusEvent, new RoutedEventHandler((s, re) => ((TextBox)s).SelectAll())); //tu: TextBox

#if _DEBUG
      //new ReportWindow(MSMoneyDbLoader.App.GetCmndLineArgsInclClickOnce())
      //new HstProcessorVw()

      //new DbLoaderReportWindow(MSMoneyDbLoader.App.GetCmndLineArgsInclClickOnce()) // 1
      //new HistoricalChartSet.MainHistChart()                // 2
      new TxCategoryAssignerVw()                            // 3
      //new ManualTxnEntry(true) //new MinFin7.DataSet.TxAdd() // 4 
      //new MinFin7.Review.DS.ReviewWindow("Mei")              // 5
      .ShowDialog();

      //MinFin7.Report.WinForm.Program.ShowBoth();
      //MinFin7.Report.WinForm.Program.Show_Alx();
      //MinFin7.Report.WinForm.Program.Show_Mei();
      //MinFin7.Report.WinForm.Program.ShowDialogBoth();
#else
    //var args = MSMoneyDbLoader.App.GetCmndLineArgsInclClickOnce(); // load cml specified file.
    //if (args.Length > 0 && (File.Exists(args[0]) || Directory.Exists(args[0])))
    //{
    //  new MSMoneyDbLoader.DbLoaderReportWindow(args).ShowDialog();
    //  if (!(args.Length > 1 && args[1].Equals("ShowMenu", System.StringComparison.OrdinalIgnoreCase)))
    //  {
    //    Synth.Speak($"{args[0]} processed. Exiting.");
    //    goto done;
    //  }
    //}

    ShutdownMode = ShutdownMode.OnExplicitShutdown;

    MainWindow = new TxCategoryAssignerVw(); // new MainAppDispatcher();
    MainWindow.ShowDialog(); 

    done:
#endif

    App.Current?.Shutdown();
  }
  protected override void OnExit(ExitEventArgs e) => base.OnExit(e);      //DateTime tbkFlt = DateTime.Now;			//Trace.WriteLine(string.Format("{0:dd HH:mm:ss} - finished; has been on for {1:N2} min.", tbkFlt, (tbkFlt - t0).TotalMinutes));
  static SpeechSynth? _synth = null;
  public static SpeechSynth Synth => _synth ??= new(new ConfigurationBuilder().AddUserSecrets<App>().Build()["AppSecrets:MagicNumber"] ?? "", true, "en-US-AriaNeural");
}
