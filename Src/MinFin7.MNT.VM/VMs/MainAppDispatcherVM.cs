namespace MinFin7.MNT.VM.VMs;
public class MainAppDispatcherVM : BaseDbVM
{
  protected List<string>? _badEmails;
  public MainAppDispatcherVM(MainVM mvm, ILogger lgr, IConfigurationRoot cfg, IBpr bpr, FinDemoContext dbx, IAddChild win, SrvrNameStore svr, DtBsNameStore dbs, GSReportStore gsr, LetDbChgStore awd, UserSettings stg, SpeechSynth sth, int oid) : base(mvm, lgr, cfg, bpr, dbx, win, svr, dbs, gsr, awd, stg, sth, oid)
  {
  }
}