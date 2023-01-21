namespace MinFin7.MNT.VM.VMs;
public partial class EmailDetailVM : BaseDbVM
{
  public EmailDetailVM(MainVM mvm, ILogger lgr, IConfigurationRoot cfg, IBpr bpr, FinDemoContext dbx, IAddChild win, UserSettings stg, SrvrNameStore svr, DtBsNameStore dbs, GSReportStore gsr, EmailOfIStore eml, LetDbChgStore awd) : base(mvm, lgr, cfg, bpr, dbx, win, svr, dbs, gsr, awd, stg, 8110)
  {
    EmailOfIStore = eml; EmailOfIStore.Changed += EmailOfIStore_Chngd;
    EmailOfI = eml.LastVal;
  }
  public override async Task<bool> InitAsync() { IsBusy = true; _ = await InitAsyncTask(EmailOfI); return await base.InitAsync(); }
  async Task<bool> InitAsyncTask(string emailOfI, [CallerMemberName] string? cmn = "")
  {
    WriteLine($"■■ Init  {GetCaller(),20}  called by  {cmn,-22} {emailOfI,-22} ■■■■");
    try
    {
      var sw = Stopwatch.StartNew();
      EmailOfI = emailOfI;

      await Task.Yield();

      return true;
    }
    catch (Exception ex) { ex.Pop(Lgr); return false; }
    finally { IsBusy = false; }
  }
  public override void Dispose()
  {
    EmailOfIStore.Changed -= EmailOfIStore_Chngd;
    base.Dispose();
  }

  async void EmailOfIStore_Chngd(string emailOfI, [CallerMemberName] string? cmn = "")
  {
    WriteLine($"■■ EmDt  {GetCaller(),20}  called by  {cmn,-22} {emailOfI,-22}  {(EmailOfI != emailOfI ? "==>Load" : "==>----")}");
    if (EmailOfI != emailOfI)
      _ = await InitAsyncTask(emailOfI);
  }
  public EmailOfIStore EmailOfIStore { get; }

  [ObservableProperty] string emailOfI = "";
}