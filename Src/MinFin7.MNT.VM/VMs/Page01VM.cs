using System.Threading;
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

      var pageCvs = (await new FinDemoDbgContextProcedures(Dbx).GroupedTxnAsync(Yoi, MatchLen)).ToList();
      PageCvs = CollectionViewSource.GetDefaultView(pageCvs);

      //PageCvs = CollectionViewSource.GetDefaultView(Dbx.Emails.Local.ToObservableCollection()); //tu: ?? instead of .LoadAsync() / .Local.ToObservableCollection() ?? === PageCvs = CollectionViewSource.GetDefaultView(await Dbx.Emails.ToListAsync());
      //PageCvs.SortDescriptions.Add(new SortDescription(nameof(Email.AddedAt), ListSortDirection.Descending));
      //PageCvs.Filter = obj => obj is not Email r || r is null ||
      //  ((string.IsNullOrEmpty(SearchText) ||
      //    r.Id.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true ||
      //    r.Notes?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true) &&
      //  (IncludeClosed == true || (string.IsNullOrEmpty(r.PermBanReason) && _badEmails is not null && !_badEmails.Contains(r.Id))));

      await GetTopDetail();
      _ = PageCvs?.MoveCurrentToFirst();

      Lgr.Log(LogLevel.Trace, GSReport = $" {PageCvs?.Cast<GroupedTxnResult>().Count():N0} / {sw.Elapsed.TotalSeconds:N1} loaded rows / s");

      Bpr.Finish(8);
      return rv;
    }
    catch (Exception ex) { ex.Pop(Lgr); return false; }
    finally { _ = await base.InitAsync(); }
  }

  [ObservableProperty] int yoi = DateTime.Today.Year - 1;
  [ObservableProperty] int matchLen = 1;
  [ObservableProperty] GroupedTxnResult? selectdEmail;

  [RelayCommand]
  async Task GetTopDetail()
  {
    await Bpr.StartAsync(8);

    try
    {
      //  if (PageCvs is null) return;

      //  var curpos = PageCvs.CurrentPosition;

      //  for (int i = 0, j = 0; i < 555 && j < 26; i++)
      //  {
      //    WriteLine($"== {i,3} {SelectdEmail?.Id}");

      await GetDetailsForSelRow();

      //    j++;

      //    if (PageCvs?.MoveCurrentToNext() != true)
      //      break;
      //  }

      //  if (curpos > 0)
      //    _ = (PageCvs?.MoveCurrentToPosition(curpos));

      Bpr.Finish(8);
    }
    catch (Exception ex) { ex.Pop(); }
  }

  async Task GetDetailsForSelRow()
  {
    if (SelectdEmail is not null)
    {
      await Bpr.TickAsync();

      //  var (ts, dd, root) = await GenderApi.CallOpenAI(Cfg, SelectdEmail.Fname ?? throw new ArgumentNullException(), true);

      //  SelectdEmail.Country = root?.country_of_origin.FirstOrDefault()?.country_name ?? root?.errmsg ?? dd ?? "?***?";
      //  SelectdEmail.Ttl_Rcvd = await Dbx.Ehists.CountAsync(r => r.EmailId == SelectdEmail.Id && r.RecivedOrSent == "R");
      //  SelectdEmail.Ttl_Sent = await Dbx.Ehists.CountAsync(r => r.EmailId == SelectdEmail.Id && r.RecivedOrSent == "S");
      //  if (SelectdEmail.Ttl_Rcvd > 0) SelectdEmail.LastRcvd = await Dbx.Ehists.Where(r => r.EmailId == SelectdEmail.Id && r.RecivedOrSent == "R").MaxAsync(r => r.EmailedAt);
      //  if (SelectdEmail.Ttl_Sent > 0) SelectdEmail.LastSent = await Dbx.Ehists.Where(r => r.EmailId == SelectdEmail.Id && r.RecivedOrSent == "S").MaxAsync(r => r.EmailedAt);
    }
  }
}