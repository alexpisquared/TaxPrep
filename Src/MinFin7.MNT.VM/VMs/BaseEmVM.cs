﻿namespace MinFin7.MNT.VM.VMs;

public partial class BaseEmVM : BaseDbVM
{
  protected List<string>? _badEmails;
  int _thisCampaign;
  public BaseEmVM(MainVM mvm, ILogger lgr, IConfigurationRoot cfg, IBpr bpr, FinDemoDbgContext dbx, IAddChild win, SrvrNameStore svr, DtBsNameStore dbs, GSReportStore gsr, LetDbChgStore awd, UserSettings stg, EmailOfIStore eml, EmailDetailVM evm, int oid) : base(mvm, lgr, cfg, bpr, dbx, win, svr, dbs, gsr, awd, stg, oid)
  {
    EmailOfIStore = eml; //EmailOfIStore.Changed += EmailOfIStore_Chngd;
    EmailOfIVM = evm;
  }

  public async override Task<bool> InitAsync()
  {
    await Task.Delay(22); // <== does not show up without this...............................


    return await base.InitAsync();
  }

  public EmailOfIStore EmailOfIStore { get; }
  public EmailDetailVM EmailOfIVM { get; }



  [RelayCommand] protected void Nxt() { Bpr.Click(); try { WriteLine(PageCvs?.MoveCurrentToNext()); } catch (Exception ex) { ex.Pop(); } }
  [RelayCommand] void OLk() { Bpr.Click(); try { _ = MessageBox.Show("■"); } catch (Exception ex) { ex.Pop(); } }
  [RelayCommand] void DNN() { Bpr.Click(); try { _ = MessageBox.Show("■"); } catch (Exception ex) { ex.Pop(); } }
}