namespace MinFin7.MNT;

public static class AppStartHelper
{
  public static void InitAppSvcs(IServiceCollection services)
  {
    _ = services.AddSingleton<IConfigurationRoot>(ConfigHelper.AutoInitConfigFromFile());

    _ = services.AddSingleton<ILogger>(sp => SeriLogHelper.CreateLogger<MainNavView>(/*Settings.Default.LogLevels*/));

    _ = services.AddSingleton<IBpr, Bpr>(); // _ = VersionHelper_.IsDbgAndRBD ? services.AddSingleton<IBpr, Bpr>() : services.AddSingleton<IBpr, BprSilentMock>();

    _ = services.AddSingleton<SpeechSynth>(s => new SpeechSynth(s.GetRequiredService<IConfigurationRoot>()["AppSecrets:MagicSpeech"] ?? "AppSecrets:MagicSpeech is missing ▄▀▄▀▄▀", true, AmbienceLib.SpeechSynth.CC.EnusAriaNeural.Voice));
  }
}