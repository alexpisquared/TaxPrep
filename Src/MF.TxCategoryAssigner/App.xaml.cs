using AAV.WPF.Helpers;
using AAV.Sys.Helpers;
using MSMoneyDbLoader;
using System.Diagnostics;
using System.IO;
using System.Speech.Synthesis;
using System.Windows;
using System.Windows.Controls;

namespace MF.TxCategoryAssigner
{
  public partial class App : Application
  {
    public static TraceSwitch AppTraceLevel_Select,
      AppTraceLevel_Config = new TraceSwitch("CfgTraceLevelSwitch", "Switch in config file:  <system.diagnostics><switches><!--0-off, 1-error, 2-warn, 3-info, 4-verbose. --><add name='CfgTraceLevelSwitch' value='3' /> "),
      AppTraceLevel_inCode = new TraceSwitch("Verbose________Trace", "This is the trace for all               messages.") { Level = TraceLevel.Info },
      AppTraceLevel_Warnng = new TraceSwitch("ErrorAndWarningTrace", "This is the trace for Error and Warning messages.") { Level = TraceLevel.Warning };

    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);
      Current.DispatcherUnhandledException += UnhandledExceptionHndlr.OnCurrentDispatcherUnhandledException;
      EventManager.RegisterClassHandler(typeof(TextBox), TextBox.GotFocusEvent, new RoutedEventHandler((s, re) => { (s as TextBox).SelectAll(); })); //tu: TextBox

#if DEBUG
      Tracer.SetupTracingOptions("MinFin", AppTraceLevel_inCode);
#else
      Tracer.SetupTracingOptions("MinFin", AppTraceLevel_Config);
#endif

#if _DEBUG
      //new ReportWindow(MSMoneyDbLoader.App.GetCmndLineArgsInclClickOnce())
      //new HstProcessorVw()

      //new DbLoaderReportWindow(MSMoneyDbLoader.App.GetCmndLineArgsInclClickOnce()) // 1
      //new HistoricalChartSet.MainHistChart()                // 2
      new TxCategoryAssignerVw()                            // 3
      //new ManualTxnEntry(true) //new MinFin.DataSet.TxAdd() // 4 
      //new MinFin.Review.DS.ReviewWindow("Mei")              // 5
      .ShowDialog();

      //MinFin.Report.WinForm.Program.ShowBoth();
      //MinFin.Report.WinForm.Program.Show_Alx();
      //MinFin.Report.WinForm.Program.Show_Mei();
      //MinFin.Report.WinForm.Program.ShowDialogBoth();
#else
      var args = MSMoneyDbLoader.App.GetCmndLineArgsInclClickOnce(); // load cml specified file.
      if (args.Length > 0 && (File.Exists(args[0]) || Directory.Exists(args[0])))
      {
        new MSMoneyDbLoader.DbLoaderReportWindow(args).ShowDialog();
        if (!(args.Length > 1 && args[1].Equals("ShowMenu", System.StringComparison.OrdinalIgnoreCase)))
        {
          Synth.Speak($"{args[0]} processed. Exiting.");
          goto done;
        }
      }

      ShutdownMode = ShutdownMode.OnExplicitShutdown;
      new Views.MainAppDispatcher().ShowDialog();

      done:
#endif

      App.Current.Shutdown();
    }
    protected override void OnExit(ExitEventArgs e) => base.OnExit(e);      //DateTime tbkFlt = DateTime.Now;			//Trace.WriteLine(string.Format("{0:dd HH:mm:ss} - finished; has been on for {1:N2} min.", tbkFlt, (tbkFlt - t0).TotalMinutes));
    static SpeechSynthesizer _synth = null; public static SpeechSynthesizer Synth { get => _synth ?? (_synth = new SpeechSynthesizer { Rate = 7 }); set => _synth = value; }
  }
}
/// CIBC: Intuit Quicken - QFX      seems to be OK (Aug 2018)
/// TDct: MS Money       - OFX
/// Amzn: gone now       - QIF      format has no card id ## in it &#10;thus, use QIF for AMAZON only, since it does not &#10;have other formats available
