namespace MinFin7.MNT.VM.VMs;

public partial class BaseEmVM : BaseDbVM
{
  protected List<string>? _badEmails;
  public BaseEmVM(MainVM mvm, ILogger lgr, IConfigurationRoot cfg, IBpr bpr, FinDemoContext dbx, IAddChild win, SrvrNameStore svr, DtBsNameStore dbs, GSReportStore gsr, LetDbChgStore awd, UserSettings stg, EmailOfIStore eml, EmailDetailVM evm, SpeechSynth sth, int oid) :
    base(mvm, lgr, cfg, bpr, dbx, win, svr, dbs, gsr, awd, stg, sth, oid)
  {
    EmailOfIStore = eml;
    EmailOfIVM = evm;
  }

  public override async Task<bool> InitAsync() => await base.InitAsync();

  public EmailOfIStore EmailOfIStore { get; }
  public EmailDetailVM EmailOfIVM { get; }

  [RelayCommand] protected void Nxt() { Bpr.Click(); try { _ = (PageCvs?.MoveCurrentToNext()); } catch (Exception ex) { ex.Pop(); } }
  [RelayCommand] /**/      void OLk() { Bpr.Click(); try { _ = MessageBox.Show("■"); } catch (Exception ex) { ex.Pop(); } }
  [RelayCommand] /**/      void DNN() { Bpr.Click(); try { _ = MessageBox.Show("■"); } catch (Exception ex) { ex.Pop(); } }
}
