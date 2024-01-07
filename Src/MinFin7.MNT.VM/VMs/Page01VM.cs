namespace MinFin7.MNT.VM.VMs;
public partial class Page01VM : BaseEmVM
{
  public Page01VM(MainVM mvm, ILogger lgr, IConfigurationRoot cfg, IBpr bpr, FinDemoContext dbx, Db.FinDemo7.Models.FinDemoContext dba, IAddChild win, UserSettings stg, SrvrNameStore svr, DtBsNameStore dbs, GSReportStore gsr, EmailOfIStore eml, LetDbChgStore awd, EmailDetailVM evm, SpeechSynth sth) :
    base(mvm, lgr, cfg, bpr, dbx, dba, win, svr, dbs, gsr, awd, stg, eml, evm, sth, 8110)
  { }
  public override async Task<bool> InitAsync()
  {
    try
    {
      IsBusy = true;
      Bpr.Start(8);
      var rv = await base.InitAsync(); _loaded = false; IsBusy = true; // or else...

      var sw = Stopwatch.StartNew();

      await Dbx.TxMoneySrc.LoadAsync();
      await Dbx.TxCategory.LoadAsync();
      await Dbx.TxCoreV2.Where(r => r.TxDate < new DateTime(YearOfIn, 1, 1)).LoadAsync();

      //2023.01.09 
      //Data binding directly to 'DbSet.Local' is not supported since it does not provide a stable ordering.
      //For WPF, bind to 'DbSet.Local.ToObservableCollection'.
      //For WinForms, bind to 'DbSet.Local.ToBindingList'.
      //For ASP.NET WebForms, bind to 'DbSet.ToList' or use Model Binding.

      TxPrevCvs = CollectionViewSource.GetDefaultView(Dbx.TxCoreV2.Local.ToObservableCollection().Where(r => r.TxDate < new DateTime(YearOfIn, 1, 1)));
      TxPrevCvs.SortDescriptions.Add(new SortDescription(nameof(TxCoreV2.CreatedAt), ListSortDirection.Descending));
      TxPrevCvs.Filter = obj => obj is not TxCoreV2 r || r is null || SelectdGrTxn is null ||
      (r.TxDate < new DateTime(YearOfIn, 1, 1) && r.TxDetail.StartsWith(SelectdGrTxn.TxDtl8));

      TxnYoiCvs = CollectionViewSource.GetDefaultView(Dbx.TxCoreV2.Where(r => r.TxDate >= new DateTime(YearOfIn, 1, 1)).ToList());
      TxnYoiCvs.SortDescriptions.Add(new SortDescription(nameof(TxCoreV2.CreatedAt), ListSortDirection.Descending));
      TxnYoiCvs.Filter = obj => obj is not TxCoreV2 r || r is null || SelectdGrTxn is null ||
      (r.TxDate >= new DateTime(YearOfIn, 1, 1) && r.TxDetail.StartsWith(SelectdGrTxn.TxDtl8));

      await LoadYoiMln();

      _ = PageCvs?.MoveCurrentToFirst();

      var ttlRows =
        PageCvs?.Cast<GroupedTxnResult>().Count() +
        TxPrevCvs?.Cast<TxCoreV2>().Count() +
        TxnYoiCvs?.Cast<TxCoreV2>().Count() +
        Dbx.TxCategory.Local.Count;
      Lgr.Log(LogLevel.Trace, GSReport = $" {ttlRows:N0} / {sw.Elapsed.TotalSeconds:N1}   (rows / s)       ");

      Bpr.Finish(8);
      return rv;
    }
    catch (Exception ex) { ex.Pop(Lgr); return false; }
    finally { _ = await base.InitAsync(); }
  }
  const int _len = 4;
  [ObservableProperty] int matchLen = _len;
  [ObservableProperty] int yearOfIn = DateTime.Today.Year - 1;
  [ObservableProperty] ICollectionView? txPrevCvs;
  [ObservableProperty] ICollectionView? txnYoiCvs;
  [ObservableProperty] ObservableCollection<string> categoriesPrevYr = new();
  [ObservableProperty][NotifyPropertyChangedFor(nameof(GSReport))] GroupedTxnResult? currentGrTxn; // demo only.
  [ObservableProperty][NotifyPropertyChangedFor(nameof(GSReport))] TxCoreV2? currentPre;
  [ObservableProperty][NotifyPropertyChangedFor(nameof(GSReport))] TxCoreV2? selectdPre;
  [ObservableProperty][NotifyPropertyChangedFor(nameof(GSReport))] TxCoreV2? currentYoi;
  [ObservableProperty][NotifyPropertyChangedFor(nameof(GSReport))] TxCoreV2? selectdYoi;
  [ObservableProperty][NotifyCanExecuteChangedFor(nameof(AssignCommand))] string? selCtgry;
  [ObservableProperty] string yearOfInStr = "2022"; partial void OnYearOfInStrChanged(string value)
  {
    Bpr.Tick();
    if (!int.TryParse(value, out var rv)) return;

    YearOfIn = rv;
    LoadYoiMlnCommand.Execute(null); //tu: async void avoidment through CMD:
  }
  [ObservableProperty] string matchLenStr = $"{_len}"; partial void OnMatchLenStrChanged(string value)
  {
    Bpr.Tick();
    if (!int.TryParse(value, out var rv)) return;

    MatchLen = rv;
    LoadYoiMlnCommand.Execute(null); //tu: async void avoidment through CMD:
  }
  [ObservableProperty] GroupedTxnResult? selectdGrTxn;

  partial void OnSelectdGrTxnChanged(GroupedTxnResult? value)
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
      PageCvs = CollectionViewSource.GetDefaultView((await new FinDemoContextProcedures(Dbx).GroupedTxnAsync(YearOfIn, MatchLen)).ToList());
      ArgumentNullException.ThrowIfNull(PageCvs);

      PageCvs.SortDescriptions.Add(new SortDescription(nameof(GroupedTxnResult.TxDtl8), ListSortDirection.Ascending));
      PageCvs.Filter = obj => obj is not GroupedTxnResult r || r is null ||
        string.IsNullOrEmpty(SearchText) || r.TxDtl8.Contains(SearchText, StringComparison.OrdinalIgnoreCase);

      Bpr.Finish(8);
    }
    catch (Exception ex) { ex.Pop(); }
  }

  [RelayCommand]
  void LoadSelTxDtl(GroupedTxnResult selGrdTx)
  {
    if (!_loaded) { return; }

    Bpr.Tick();
    try
    {
      TxPrevCvs?.Refresh();

      var filteredItems = TxPrevCvs?.Cast<TxCoreV2>();
      if (filteredItems != null)
      {
        CategoriesPrevYr.Clear();
        filteredItems.Select(r => r.TxCategoryIdTxt).Distinct().OrderBy(r => r).ToList().ForEach(CategoriesPrevYr.Add);
      }

      if (CategoriesPrevYr.Count == 1)
        SelCtgry = CategoriesPrevYr.FirstOrDefault();

      TxnYoiCvs?.Refresh();
      TxnYoiCvs?.MoveCurrentTo(null);
      Bpr.Click();
    }
    catch (Exception ex) { ex.Pop(); }
  }

  [RelayCommand(CanExecute = nameof(CanAssign))]
  async void Assign(string? categoryIdTxt)
  {
    try
    {
      var now = DateTime.Now;

      if (SelectdYoi is null)
      {
        TxnYoiCvs?.Cast<TxCoreV2>().Where(r => r.TxCategoryIdTxt == "UnKn").ToList().ForEach(r =>
        {
          r.TxCategoryIdTxt = categoryIdTxt;
          r.ModifiedAt = now;
          r.Notes += " )■*";
        });

        TxnYoiCvs?.Refresh();
        await Sth.SpeakAsync("Look it!");
        await Bpr.BeepAsync(333, .333); // calming down demo of what's done.

        Nxt();
      }
      else
      {
        SelectdYoi.TxCategoryIdTxt = categoryIdTxt;
        SelectdYoi.ModifiedAt = now;
        SelectdYoi.Notes += " )■*";

        TxnYoiCvs?.Refresh();
        Bpr.Click();
      }

      Save2DbCommand.Execute(null); //fallback JIC: ChkDb4CngsCommand.Execute(null);
    }
    catch (Exception ex) { ex.Pop(); }
  }
  static bool CanAssign(string? selctgry) => selctgry is not null; // https://learn.microsoft.com/en-us/dotnet/communitytoolkit/mvvm/generators/relaycommand

  [RelayCommand] void Cou() { Bpr.Click(); try { Nxt(); } catch (Exception ex) { ex.Pop(); } }
  [RelayCommand] void PBR() { Bpr.Click(); try { Nxt(); } catch (Exception ex) { ex.Pop(); } }
  [RelayCommand] void Del() { Bpr.Click(); try { Nxt(); } catch (Exception ex) { ex.Pop(); } }
}