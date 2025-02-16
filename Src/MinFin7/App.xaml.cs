using Azure.Identity;
using static AmbienceLib.SpeechSynth;

using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Speech.Synthesis;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Azure.Security.KeyVault.Secrets;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Configuration;
using static System.Diagnostics.Trace;
using Application = System.Windows.Application;


namespace MinFin7;
public partial class App : Application
{
  static readonly DateTime Started = DateTime.Now;
  //static readonly IConfigurationRoot? _config = ConfigHelper.AutoInitConfigFromFile();
  ILogger<TxCategoryAssignerVw>? _logger;
  protected override void OnStartup(StartupEventArgs e)
  {
    base.OnStartup(e);

    _logger = (ILogger<TxCategoryAssignerVw>?)SeriLogHelper.InitLoggerFactory(/*@$"C:\Temp\Logs\{Assembly.GetExecutingAssembly().GetName().Name![..5]}.{VersionHelper.Env()}.{Environment.UserName[..3]}..log", "+Info"*/).CreateLogger<TxCategoryAssignerVw>();

    Current.DispatcherUnhandledException += UnhandledExceptionHndlrUI.OnCurrentDispatcherUnhandledException;
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

    ShutdownMode = ShutdownMode.OnMainWindowClose; // The default value is OnLastWindowClose.

    var connectionString = new ConfigurationBuilder().AddUserSecrets<App>().Build()["ConnectionStrings:default"] ?? throw new ArgumentNullException("■ !Config - ConnectionStrings:Express"); //tu: adhoc usersecrets from config

    try
    {
      var client = new SecretClient(new Uri("https://demopockv.vault.azure.net/"), new DefaultAzureCredential());
      KeyVaultSecret secret = client.GetSecret("FreeDbConStr");
      connectionString = secret.Value;
    }
    catch (Exception ex)
    {
      Trace.WriteLine(ex.Message);
      connectionString = new ConfigurationBuilder().AddUserSecrets<App>().Build()["ConnectionStrings:default"] ?? throw new ArgumentNullException("■ !Config - ConnectionStrings:Express"); //tu: adhoc usersecrets from config
    }

    MainWindow =
    new TxCategoryAssignerVw(_logger, new Bpr(), Synth, new FinDemoContext(connectionString));
    //VersionHelper.IsDbg ?      
    //new ManualTxnEntry(_logger, new Bpr(), Synth, true, new FinDemoContext(constr));
    //new MinFin7MdiLib.Views.ReviewWindow(_logger, new Bpr(), "Mei", new FinDemoContext(constr));
    //new MainAppDispatcher();// (_logger, new Bpr(), Synth, new FinDemoContext(constr));

    MainWindow.ShowDialog();

    //done:
#endif

    //Current?.Shutdown();
  }
  protected override void OnExit(ExitEventArgs e) => base.OnExit(e);      //DateTime tbkFlt = DateTime.Now;			//Trace.WriteLine(string.Format("{0:dd HH:mm:ss} - finished; has been on for {1:N2} min.", tbkFlt, (tbkFlt - t0).TotalMinutes));
  static SpeechSynth? _sy = null; public static SpeechSynth Synth => _sy ??= new(new ConfigurationBuilder().AddUserSecrets<App>().Build()["AppSecrets:MagicSpeech"] ?? "AppSecrets:MagicSpeech is missing ▄▀▄▀▄▀", true, CC.EnusAriaNeural.Voice);
}
