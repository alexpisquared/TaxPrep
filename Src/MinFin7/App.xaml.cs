namespace MinFin7;
public partial class App : Application
{
  protected override void OnStartup(StartupEventArgs e)
  {
    base.OnStartup(e);

    var _logger = (ILogger<TxCategoryAssignerVw>?)SeriLogHelper.InitLoggerFactory(/*@$"C:\Temp\Logs\{Assembly.GetExecutingAssembly().GetName().Name![..5]}.{VersionHelper.Env()}.{Environment.UserName[..3]}..log", "+Info"*/).CreateLogger<TxCategoryAssignerVw>();

    Current.DispatcherUnhandledException += UnhandledExceptionHndlrUI.OnCurrentDispatcherUnhandledException;
    EventManager.RegisterClassHandler(typeof(TextBox), UIElement.GotFocusEvent, new RoutedEventHandler((s, re) => ((TextBox)s).SelectAll())); //tu: TextBox

    ShutdownMode = ShutdownMode.OnMainWindowClose; // The default value is OnLastWindowClose.

    var config = new ConfigurationBuilder().AddUserSecrets<App>().Build(); //tu: ad-hoc user Secrets from config

    MainWindow = new TxCategoryAssignerVw(_logger!, new Bpr(), new SpeechSynth(config["AppSecrets:MagicSpeech"] ?? throw new ArgumentNullException("■ !Config: MagicSpeech"), true, CC.EnusAriaNeural.Voice), new FinDemoContext(GetConnectionString(_logger, config)));
    MainWindow.ShowDialog();
  }

  private static string GetConnectionString(ILogger<TxCategoryAssignerVw>? _logger, IConfigurationRoot config)
  {
    try
    {
      return new SecretClient(
        new Uri(config["akv:KvUrl"]!),
        new ClientSecretCredential(config["akv:DirId"], config["akv:AppId"], config["akv:SeVal"])).
        GetSecret(config["akv:SName"]).Value.Value;
    }
    catch (Exception ex)
    {
      _logger?.LogError(ex, "■");
      return config["ConnectionStrings:default"] ?? throw new ArgumentNullException(nameof(config), "■ !Config: ConnectionStrings:default");
    }
  }
}