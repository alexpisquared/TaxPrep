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
      //await Dbx.Phones.LoadAsync();

      await LoadYoiMln();


      _ = PageCvs?.MoveCurrentToFirst();

      Lgr.Log(LogLevel.Trace, GSReport = $" {PageCvs?.Cast<GroupedTxnResult>().Count():N0} / {sw.Elapsed.TotalSeconds:N1} loaded rows / s");

      Bpr.Finish(8);
      return rv;
    }
    catch (Exception ex) { ex.Pop(Lgr); return false; }
    finally { _ = await base.InitAsync(); }
  }


  [ObservableProperty] int matchLen = 16;
  [ObservableProperty] int yearOfIn = DateTime.Today.Year - 1;
  [ObservableProperty][NotifyPropertyChangedFor(nameof(GSReport))] GroupedTxnResult? currentEmail; // demo only.
  [ObservableProperty] string yearOfInStr = "2022"; partial void OnYearOfInStrChanged(string value)
  {
    Bpr.Tick();
    if (!int.TryParse(value, out var rv)) return;

    YearOfIn = rv;
    LoadYoiMlnCommand.Execute(null); //tu: async void avoidment through CMD:
  }
  [ObservableProperty] string matchLenStr = "4"; partial void OnMatchLenStrChanged(string value)
  {
    Bpr.Tick();
    if (!int.TryParse(value, out var rv)) return;

    MatchLen = rv;
    LoadYoiMlnCommand.Execute(null); //tu: async void avoidment through CMD:
  }
  [ObservableProperty] GroupedTxnResult? selectdEmail; partial void OnSelectdEmailChanged(GroupedTxnResult? value)
  {
    Bpr.Tick(); ;
    LoadSelTxDtlCommand.Execute(value); //tu: async void avoidment through CMD:
  }

  [RelayCommand]
  async Task LoadYoiMln()
  {
    Bpr.Start(8);
    try
    {
      PageCvs = CollectionViewSource.GetDefaultView((await new FinDemoDbgContextProcedures(Dbx).GroupedTxnAsync(YearOfIn, MatchLen)).ToList());
      ArgumentNullException.ThrowIfNull(PageCvs);

      PageCvs.SortDescriptions.Add(new SortDescription(nameof(GroupedTxnResult.TxDtl8), ListSortDirection.Descending));
      PageCvs.Filter = obj => obj is not GroupedTxnResult r || r is null ||
        ((string.IsNullOrEmpty(SearchText) ||
          r.TxDtl8.Contains(SearchText, StringComparison.OrdinalIgnoreCase) == true));

      Bpr.Finish(8);
    }
    catch (Exception ex) { ex.Pop(); }
  }

  [RelayCommand]
  async Task LoadSelTxDtl(GroupedTxnResult selectdEmail)
  {
    Bpr.Start(8);
    try
    {

      await Bpr.FinishAsync(8);
    }
    catch (Exception ex) { ex.Pop(); }
  }
}