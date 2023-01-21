using MinFin7.MNT.View.Main;

namespace MinFin7.MNT;

public static class AppStartHelper
{
  public static void InitAppSvcs(IServiceCollection services)
  {
    _ = services.AddSingleton<IConfigurationRoot>(ConfigHelper.AutoInitConfigFromFile());

    _ = services.AddSingleton<ILogger>(sp => SeriLogHelper.InitLoggerFactory(
      folder: FSHelper.GetCreateSafeLogFolderAndFile((sp.GetRequiredService<IConfigurationRoot>()[CfgName.LogFolder] ?? "Logs").Replace("..", $"{Assembly.GetExecutingAssembly().GetName().Name![..6]}.{VersionHelper.Env()}.{Environment.UserName[..3]}..")),
      levels: "+Verbose -Info -Warning +Error -ErNT -11mb -Infi").CreateLogger<MainNavView>());

    _ = services.AddSingleton<IBpr, Bpr>(); // _ = VersionHelper_.IsDbgAndRBD ? services.AddSingleton<IBpr, Bpr>() : services.AddSingleton<IBpr, BprSilentMock>();

    _ = services.AddSingleton<SpeechSynth>(s => new SpeechSynth(s.GetRequiredService<IConfigurationRoot>()["AppSecrets:MagicSpeech"] ?? "AppSecrets:MagicSpeech is missing ▄▀▄▀▄▀", true, CC.EnusAriaNeural.Voice));
  }
}