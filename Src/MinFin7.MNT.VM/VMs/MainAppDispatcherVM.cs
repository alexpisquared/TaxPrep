namespace MinFin7.MNT.VM.VMs;
public partial class MainAppDispatcherVM : BaseDbVM
{
  public MainAppDispatcherVM(MainVM mvm, ILogger lgr, IConfigurationRoot cfg, IBpr bpr, FinDemoContext dbx, IAddChild win, SrvrNameStore svr, DtBsNameStore dbs, GSReportStore gsr, LetDbChgStore awd, UserSettings stg, SpeechSynth sth) :
    base(mvm, lgr, cfg, bpr, dbx, win, svr, dbs, gsr, awd, stg, sth, 8880)
  { }

  [ObservableProperty] string justForLaughs = "Explore Src Folder";
}