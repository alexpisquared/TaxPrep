namespace MinFin7.MNT.VM.VMs;
public partial class Page00VM : BaseDbVM
{
  readonly DateTimeOffset _now = DateTimeOffset.Now;
  public Page00VM(MainVM mvm, ILogger lgr, IConfigurationRoot cfg, IBpr bpr, FinDemoContext dbx, Db.FinDemo7.Models.FinDemoContext dba, IAddChild win, UserSettings stg, SrvrNameStore svr, DtBsNameStore dbs, GSReportStore gsr, LetDbChgStore awd, SpeechSynth sth) : 
    base(mvm, lgr, cfg, bpr, dbx, dba, win, svr, dbs, gsr, awd, stg, sth, 8110) => _ = Application.Current.Dispatcher.InvokeAsync(async () => { try { await Task.Yield(); } catch (Exception ex) { ex.Pop(Lgr); } });    //tu: async prop - https://stackoverflow.com/questions/6602244/how-to-call-an-async-method-from-a-getter-or-setter
  public override async Task<bool> InitAsync()
  {
    try
    {
      Cfg[CfgName.ServerLst]?.Split(" ", StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(SqlServrs.Add);
      Cfg[CfgName.DtBsNmLst]?.Split(" ", StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(DtBsNames.Add);

      SrvrName = UsrStgns.SrvrName;
      DtBsName = UsrStgns.DtBsName;

      return true;
    }
    catch (Exception ex) { ex.Pop(Lgr); return false; }
    finally { _ = await base.InitAsync(); }
  }
  public override Task<bool> WrapAsync() => base.WrapAsync();

  public List<string> SqlServrs { get; } = new();
  public List<string> DtBsNames { get; } = new();

  [RelayCommand] async Task ImportCsv() => await new CsvImporterService(Dbx, Lgr, _now).ImportCsvAsync(ReportProgress);
}