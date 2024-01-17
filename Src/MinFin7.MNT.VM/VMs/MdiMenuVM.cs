namespace MinFin7.MNT.VM.VMs;
public partial class MdiMenuVM : BaseDbVM
{
  public MdiMenuVM(MainVM mvm, ILogger lgr, IConfigurationRoot cfg, IBpr bpr, FinDemoContext dbx, Db.FinDemo7.Models.FinDemoContext dba, IAddChild win, SrvrNameStore svr, DtBsNameStore dbs, GSReportStore gsr, LetDbChgStore awd, UserSettings stg, SpeechSynth sth) :
    base(mvm, lgr, cfg, bpr, dbx, dba, win, svr, dbs, gsr, awd, stg, sth, 8880)
  { }

  [ObservableProperty] string justForLaughs = "Explore Src Folder";
}