namespace MinFin7.MNT.VM.VMs;
public partial class MainVM : BaseMinVM
{
  readonly NavigationStore _navigationStore;
  public MainVM(NavBarVM navBarVM, NavigationStore navigationStore, ILogger lgr, IBpr bpr, IConfigurationRoot cfg, SrvrNameStore svr, DtBsNameStore dbs, GSReportStore gsr, EmailOfIStore eml, LetDbChgStore alw, UserSettings usr) : base()
  {
    NavBarVM = navBarVM;
    _navigationStore = navigationStore;
    Lgr = lgr;
    Bpr = bpr;
    UsrStgns = usr;

    IsDevDbg = VersionHelper.IsDbg;

    _navigationStore.CurrentVMChanged += OnCurrentVMChanged;

    SrvrNameStore = svr; SrvrNameStore.Changed += SrvrNameStore_Chngd;
    DtBsNameStore = dbs; DtBsNameStore.Changed += DtBsNameStore_Chngd;
    GSReportStore = gsr; GSReportStore.Changed += GSReportStore_Chngd;
    EmailOfIStore = eml; EmailOfIStore.Changed += EmailOfIStore_Chngd;
    _letDbChStore = alw; _letDbChStore.Changed += LetDbChgStore_Chngd;

    cfg[CfgName.ServerLst]?.Split(" ", StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(r => SqlServrs.Add(r));
    cfg[CfgName.DtBsNmLst]?.Split(" ", StringSplitOptions.RemoveEmptyEntries).ToList().ForEach(r => DtBsNames.Add(r));

    Bpr.SuppressTicks = Bpr.SuppressAlarm = !(IsAudible = UsrStgns.IsAudible);
    IsAnimeOn = UsrStgns.IsAnimeOn;

    Bpr.Start();

    _ctored = true;
  }
  public override async Task<bool> InitAsync()
  {
    SrvrName = UsrStgns.SrvrName;
    DtBsName = UsrStgns.DtBsName;
    EmailOfI = UsrStgns.EmailOfI;
    LetDbChg = UsrStgns.LetDbChg;

    AppVerNumber = VersionHelper.CurVerStr;
    AppVerToolTip = VersionHelper.CurVerStr;

    return await base.InitAsync();
  }
  public override void Dispose()
  {
    NavBarVM.Dispose();

    _navigationStore.CurrentVMChanged -= OnCurrentVMChanged;

    SrvrNameStore.Changed -= SrvrNameStore_Chngd;
    DtBsNameStore.Changed -= DtBsNameStore_Chngd;
    GSReportStore.Changed -= GSReportStore_Chngd;
    EmailOfIStore.Changed -= EmailOfIStore_Chngd;
    _letDbChStore.Changed -= LetDbChgStore_Chngd;

    base.Dispose();
  }

  protected readonly LetDbChgStore _letDbChStore;
  public SrvrNameStore SrvrNameStore { get; }
  public DtBsNameStore DtBsNameStore { get; }
  public GSReportStore GSReportStore { get; }
  public EmailOfIStore EmailOfIStore { get; }
  void SrvrNameStore_Chngd(string val) { SrvrName = val;   /* await RefreshReloadAsync() */; }
  void DtBsNameStore_Chngd(string val) { DtBsName = val;   /* await RefreshReloadAsync() */; }
  void GSReportStore_Chngd(string val) { GSReport = val;   /* await RefreshReloadAsync() */; }
  void EmailOfIStore_Chngd(string val, [CallerMemberName] string? cmn = "") { Lgr.Log(LogLevel.Trace, $"■■ MAIN  {GetCaller(),20}  called by  {cmn,-22} {val,-22}  {(EmailOfI != val ? "==>Load as it were ..." : "==>----")}"); EmailOfI = val;   /* await RefreshReloadAsync(); */  }
  void LetDbChgStore_Chngd(bool value) { LetDbChg = value; /* await RefreshReloadAsync() */; }
  string _qs = ""; public string SrvrName { get => _qs; set { if (SetProperty(ref _qs, value, true) && value is not null && _loaded) { Bpr.Tick(); SrvrNameStore.Change(value); UsrStgns.SrvrName = value; } } }
  string _dn = ""; public string DtBsName { get => _dn; set { if (SetProperty(ref _dn, value, true) && value is not null && _loaded) { Bpr.Tick(); DtBsNameStore.Change(value); UsrStgns.DtBsName = value; } } }
  string _gr = ""; public string GSReport { get => _gr; set { if (SetProperty(ref _gr, value, true) && value is not null && _loaded) { /*       */ GSReportStore.Change(value); GSRepViz = string.IsNullOrWhiteSpace(value) ? Visibility.Collapsed : Visibility.Visible; } } }
  string _em = ""; public string EmailOfI { get => _em; set { if (SetProperty(ref _em, value, true) && value is not null && _loaded) { Bpr.Tick(); EmailOfIStore.Change(value); UsrStgns.EmailOfI = value; } } }
  bool _au; public bool IsAudible { get => _au; set { if (SetProperty(ref _au, value, true) && _ctored) { Bpr.Tick(); Bpr.SuppressTicks = Bpr.SuppressAlarm = !(UsrStgns.IsAudible = value); Lgr.LogInformation($"│   user-pref-auto-poll:       IsAudible: {value} ■─────■"); } } }
  bool _an; public bool IsAnimeOn { get => _an; set { if (SetProperty(ref _an, value, true) && _ctored) { Bpr.Tick(); UsrStgns.IsAnimeOn = value; Lgr.LogInformation($"│   user-pref-auto-poll:       IsAnimeOn: {value} ■─────■"); } } }
  bool _aw; public bool LetDbChg { get => _aw; set { if (SetProperty(ref _aw, value, true) && _loaded) { Bpr.Tick(); UsrStgns.LetDbChg = value; _letDbChStore.Change(value); } } }

  public IBpr Bpr { get; }
  public ILogger Lgr { get; }
  public UserSettings UsrStgns { get; }
  public NavBarVM NavBarVM { get; }
  public BaseMinVM? CurrentVM => _navigationStore.CurrentVM;
  public List<string> SqlServrs { get; } = new();
  public List<string> DtBsNames { get; } = new();


  [ObservableProperty] double upgradeUrgency = 1;         // in days
  [ObservableProperty] string appVerNumber = "0.0";
  [ObservableProperty] object appVerToolTip = "Old";
  [ObservableProperty] string busyMessage = "Loading...";
  [ObservableProperty] bool isDevDbg;
  [ObservableProperty] bool isObsolete;
  [ObservableProperty] int navAnmDirn;
  [ObservableProperty] Visibility isDevDbgViz = Visibility.Visible;
  [ObservableProperty] Visibility gSRepViz = Visibility.Visible;
  //[ObservableProperty] bool isBusy;// /*BusyBlur = value ? 8 : 0;*/  }
  [ObservableProperty] ObservableCollection<string?> validationMessages = new();

  void OnCurrentVMChanged() => OnPropertyChanged(nameof(CurrentVM));
  void OnCurrentModalVMChanged()
  {
    //OnPropertyChanged(nameof(CurrentModalVM));
    //OnPropertyChanged(nameof(IsOpen));
  }

  [RelayCommand]
  async Task CloseApp()
  {
    var allowExit = await base.WrapAsync();
    if (allowExit)
    {
      Application.Current.Shutdown();
    }
  }
  [RelayCommand] async Task UpgradeSelf() { await Task.Yield(); ; }
  [RelayCommand]
  void HidePnl()
  {
    GSReport = "";
    GSRepViz = Visibility.Collapsed;
  }
}