using StandardLib.Helpers;

namespace MinFin7.MNT.VM.VMs;
public partial class Page01VM : BaseEmVM
{
  public Page01VM(MainVM mvm, ILogger lgr, IConfigurationRoot cfg, IBpr bpr, FinDemoDbgContext dbx, IAddChild win, UserSettings stg, SrvrNameStore svr, DtBsNameStore dbs, GSReportStore gsr, EmailOfIStore eml, LetDbChgStore awd, EmailDetailVM evm) : base(mvm, lgr, cfg, bpr, dbx, win, svr, dbs, gsr, awd, stg, eml, evm, 8110) { }
  public async override Task<bool> InitAsync()
  {
    try
    {
      IsBusy = true;
      Bpr.Start(8);
      await Task.Delay(2); // <== does not show up without this...............................
      var rv = await base.InitAsync(); _loaded = false; IsBusy = true; // or else...

      var sw = Stopwatch.StartNew();
      //await Dbx.PhoneEmailXrefs.LoadAsync();
      //await Dbx.Phones.LoadAsync();

      //await Dbx.Emails.LoadAsync();
      //PageCvs = CollectionViewSource.GetDefaultView(Dbx.Emails.Local.ToObservableCollection()); //tu: ?? instead of .LoadAsync() / .Local.ToObservableCollection() ?? === PageCvs = CollectionViewSource.GetDefaultView(await Dbx.Emails.ToListAsync());
      //PageCvs.SortDescriptions.Add(new SortDescription(nameof(Email.AddedAt), ListSortDirection.Descending));
      //PageCvs.Filter = obj => obj is not Email r || r is null ||
      //  ((string.IsNullOrEmpty(SearchText) ||
      //    r.Id.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true ||
      //    r.Notes?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true) &&
      //  (IncludeClosed == true || (string.IsNullOrEmpty(r.PermBanReason) && _badEmails is not null && !_badEmails.Contains(r.Id))));

      //await GetTopDetail();
      _ = PageCvs?.MoveCurrentToFirst();

      //Lgr.Log(LogLevel.Trace, GSReport = $" {PageCvs?.Cast<Email>().Count():N0} / {Dbx.Emails.Local.Count:N0} / {sw.Elapsed.TotalSeconds:N1} loaded rows / s");

      Bpr.Finish(8);
      return rv;
    }
    catch (Exception ex) { ex.Pop(Lgr); return false; }
    finally { _ = await base.InitAsync(); }
  }
}
