namespace MinFin7.MNT.VM.VMs;
public partial class Page01VM : BaseEmVM
{
    public Page01VM(MainVM mvm, ILogger lgr, IConfigurationRoot cfg, IBpr bpr, FinDemoDbgContext dbx, IAddChild win, UserSettings stg, SrvrNameStore svr, DtBsNameStore dbs, GSReportStore gsr, EmailOfIStore eml, LetDbChgStore awd, EmailDetailVM evm)
      : base(mvm, lgr, cfg, bpr, dbx, win, svr, dbs, gsr, awd, stg, eml, evm, 8110) { }

}