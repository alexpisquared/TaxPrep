namespace MinFin7.MNT.VM.VMs;
public partial class Page01VM : BaseEmVM
{
  public Page01VM(MainVM mvm, ILogger lgr, IConfigurationRoot cfg, IBpr bpr, FinDemoDbgContext dbx, IAddChild win, UserSettings stg, SrvrNameStore svr, DtBsNameStore dbs, GSReportStore gsr, EmailOfIStore eml, LetDbChgStore awd, EmailDetailVM evm) : base(mvm, lgr, cfg, bpr, dbx, win, svr, dbs, gsr, awd, stg, eml, evm, 8110) { }
  public override async Task<bool> InitAsync()
  {
    try
    {
      IsBusy = true;
      Bpr.Start(8);
      var rv = await base.InitAsync(); _loaded = false; IsBusy = true; // or else...

      var sw = Stopwatch.StartNew();

      await Dbx.TxCategory.LoadAsync();
      await Dbx.TxCoreV2.LoadAsync();

      TxCoCvs = CollectionViewSource.GetDefaultView(Dbx.TxCoreV2.Local.ToObservableCollection()); //tu: ?? instead of .LoadAsync() / .Local.ToObservableCollection() ?? === PageCvs = CollectionViewSource.GetDefaultView(await Dbx.TxCoreV2.ToListAsync());
      TxCoCvs.SortDescriptions.Add(new SortDescription(nameof(TxCoreV2.CreatedAt), ListSortDirection.Descending));
      TxCoCvs.Filter = obj => obj is not TxCoreV2 r || r is null || SelectdGrTxn is null ||
      (r.TxDate < new DateTime(YearOfIn, 1, 1) && r.TxDetail.StartsWith(SelectdGrTxn.TxDtl8));

      TxcNCvs = CollectionViewSource.GetDefaultView(Dbx.TxCoreV2.ToList());
      TxcNCvs.SortDescriptions.Add(new SortDescription(nameof(TxCoreV2.CreatedAt), ListSortDirection.Descending));
      TxcNCvs.Filter = obj => obj is not TxCoreV2 r || r is null || SelectdGrTxn is null ||
      (r.TxDate >= new DateTime(YearOfIn, 1, 1) && r.TxDetail.StartsWith(SelectdGrTxn.TxDtl8));

      await LoadYoiMln();

      _ = PageCvs?.MoveCurrentToFirst();

      var ttlRows =
        PageCvs?.Cast<GroupedTxnResult>().Count() +
        TxCoCvs?.Cast<TxCoreV2>().Count() +
        TxcNCvs?.Cast<TxCoreV2>().Count() +
        Dbx.TxCategory.Local.Count;
      Lgr.Log(LogLevel.Trace, GSReport = $" {ttlRows:N0} / {sw.Elapsed.TotalSeconds:N1}   (rows / s)       ");

      Bpr.Finish(8);
      return rv;
    }
    catch (Exception ex) { ex.Pop(Lgr); return false; }
    finally { _ = await base.InitAsync(); }
  }

  [ObservableProperty] int matchLen = 16;
  [ObservableProperty] int yearOfIn = DateTime.Today.Year - 1;
  [ObservableProperty][NotifyPropertyChangedFor(nameof(GSReport))] GroupedTxnResult? currentEmail; // demo only.
  [ObservableProperty][NotifyPropertyChangedFor(nameof(GSReport))] TxCoreV2? currentTxCo;
  [ObservableProperty][NotifyPropertyChangedFor(nameof(GSReport))] TxCoreV2? selectdTxCo;
  [ObservableProperty] ICollectionView? txCoCvs;
  [ObservableProperty] ICollectionView? txcNCvs;
  [ObservableProperty] ObservableCollection<string> categoriesPrevYr = new();
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
  [ObservableProperty] GroupedTxnResult? selectdGrTxn; partial void OnSelectdGrTxnChanged(GroupedTxnResult? value)
  {
    Bpr.Tick();
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

      PageCvs.SortDescriptions.Add(new SortDescription(nameof(GroupedTxnResult.TxDtl8), ListSortDirection.Ascending));
      PageCvs.Filter = obj => obj is not GroupedTxnResult r || r is null ||
        string.IsNullOrEmpty(SearchText) || r.TxDtl8.Contains(SearchText, StringComparison.OrdinalIgnoreCase);

      Bpr.Finish(8);
    }
    catch (Exception ex) { ex.Pop(); }
  }

  [RelayCommand]
  async Task LoadSelTxDtl(GroupedTxnResult selGrdTx)
  {
    if (!_loaded) { return; }

    Bpr.Start(8);
    try
    {
      TxCoCvs?.Refresh();

      var filteredItems = TxCoCvs?.Cast<TxCoreV2>();
      if (filteredItems != null)
      {
        CategoriesPrevYr.Clear();
        filteredItems.Select(r => r.TxCategoryIdTxt).Distinct().OrderBy(r => r).ToList().ForEach(CategoriesPrevYr.Add);
      }

      TxcNCvs?.Refresh();
      TxcNCvs?.MoveCurrentTo(null);

      await Bpr.FinishAsync(8);
    }
    catch (Exception ex) { ex.Pop(); }
  }

  [RelayCommand] void Cou() { Bpr.Click(); try { Nxt(); } catch (Exception ex) { ex.Pop(); } }
  [RelayCommand] void PBR() { Bpr.Click(); try { Nxt(); } catch (Exception ex) { ex.Pop(); } }
  [RelayCommand] void Del() { Bpr.Click(); try { Nxt(); } catch (Exception ex) { ex.Pop(); } }
}