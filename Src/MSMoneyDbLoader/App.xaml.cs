using AAV.Sys.Ext;
using AsLink;
using System;
using System.Speech.Synthesis;
using System.Windows;
using System.Windows.Controls;

namespace MSMoneyDbLoader
{
  public partial class App : Application
  {
    static SpeechSynthesizer _synth = null; public static SpeechSynthesizer Synth { get => _synth ?? (_synth = new SpeechSynthesizer { Rate = 5 }); set => _synth = value; }

    protected override void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);

      //Application.Current.DispatcherUnhandledException += UnhandledExceptionHndlr.OnCurrentDispatcherUnhandledException;
      EventManager.RegisterClassHandler(typeof(TextBox), TextBox.GotFocusEvent, new RoutedEventHandler((s, re) => { (s as TextBox).SelectAll(); })); //tu: TextBox
      //DevOpStartup.SetupTracingOptions("MSMoneyDbLoader.");

      try
      {
        var vw =
          new DbLoaderReportWindow(GetCmndLineArgsInclClickOnce());
        //new HistoricalChartSet.MainHistChart();
        vw.ShowDialog();
      }
      catch (Exception ex) { ex.Log(); }

      App.Current.Shutdown();
    }
    protected override void OnExit(ExitEventArgs e) => base.OnExit(e); //DateTime t1 = DateTime.Now;			//Trace.WriteLine(string.Format("{0:dd HH:mm:ss} - finished; has been on for {1:N2} min.", t1, (t1 - t0).TotalMinutes));

    public static string[] GetCmndLineArgsInclClickOnce()
    {
      string[] args = Array.Empty<string>();

      try
      {
        if (AppDomain.CurrentDomain.SetupInformation.ActivationArguments != null &&                  //first, check the ClickOnce args
          AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData != null &&
          AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData.Length > 0)
        {
          args = new string[AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData.Length];
          for (var i = 0; i < AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData.Length; i++)
            args[i] = Uri.UnescapeDataString(AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData[i]).Replace("file:///", "").Replace("/", "\\");
        }
        else if (Environment.GetCommandLineArgs() != null && Environment.GetCommandLineArgs().Length > 1)
        {
          args = new string[Environment.GetCommandLineArgs().Length - 1];
          for (var i = 0; i < Environment.GetCommandLineArgs().Length - 1; i++)
            args[i] = Environment.GetCommandLineArgs()[i + 1];
        }
        else
          args = new string[0];
      }
      catch (Exception ex) { ex.Log(); }

      return args;
    }

  }
}
