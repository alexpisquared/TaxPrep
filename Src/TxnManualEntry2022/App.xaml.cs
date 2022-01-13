using Microsoft.Extensions.Configuration;

namespace TxnManualEntry2022;

public partial class App : Application
{
  readonly IHost _host;

  public App() => _host = Host.CreateDefaultBuilder()
    //.ConfigureAppConfiguration((builder) => { 
    //  //builder.Configuration. 
    //  })      
    .ConfigureServices((hostContext, services) =>
    {
      var ConnectionString = hostContext.Configuration.GetConnectionString("Default");

      services.AddSingleton<TmeDbContextFactory>(new TmeDbContextFactory(ConnectionString));          //_tmeDbContextFactory = new TmeDbContextFactory(ConnectionString);
      services.AddSingleton<IReservationProvider, DatabaseReservationProvider>();                     //IReservationProvider reservationProvider = new DatabaseReservationProvider(_tmeDbContextFactory);
      services.AddSingleton<IReservationCreator, DatabaseReservationCreator>();                       //IReservationCreator reservationCreator = new DatabaseReservationCreator(_tmeDbContextFactory);
      services.AddSingleton<IReservationConflictValidator, DatabaseReservationConflictValidator>();   //IReservationConflictValidator reservationConflictValidator = new DatabaseReservationConflictValidator(_tmeDbContextFactory); 
      services.AddTransient<TxnBook>();                                                               // TxnBook txnBook = new TxnBook(reservationProvider, reservationCreator, reservationConflictValidator);
      services.AddSingleton<BankAccount>((s) => new BankAccount("CIBC Viza", s.GetRequiredService<TxnBook>()));  //_bankAccount = new BankAccount("CIBC Viza", txnBook);

      services.AddTransient((s) => CreateAcntTxnListingVM(s));
      services.AddSingleton<Func<AcntTxnListingVM>>((s) => () => s.GetRequiredService<AcntTxnListingVM>());
      services.AddSingleton<NavigationService<AcntTxnListingVM>>();

      services.AddTransient<MakeAcntTxnVM>();
      services.AddSingleton<Func<MakeAcntTxnVM>>((s) => () => s.GetRequiredService<MakeAcntTxnVM>()); // https://youtu.be/dgJ1nS2CLpQ?list=PLA8ZIAm2I03hS41Fy4vFpRw8AdYNBXmNm&t=549
      services.AddSingleton<NavigationService<MakeAcntTxnVM>>();

      services.AddSingleton<BankAccountStore>();                                                      //_bankAccountStore = new BankAccountStore(_bankAccount);
      services.AddSingleton<NavigationStore>();                                                       //_navigationStore = new NavigationStore();

      services.AddSingleton<MainVM>();
      services.AddSingleton(s => new MainWindow() { DataContext = s.GetRequiredService<MainVM>() });

    }).Build();

  AcntTxnListingVM CreateAcntTxnListingVM(IServiceProvider s) =>
    AcntTxnListingVM.LoadViewModel(
      s.GetRequiredService<BankAccountStore>(),
      s.GetRequiredService<NavigationService<MakeAcntTxnVM>>());

  protected override void OnStartup(StartupEventArgs e)
  {
    _host.Start();

    //DbContextOptions options = new DbContextOptionsBuilder().UseSqlite(ConnectionString).Options;
    using (var dbContext = _host.Services.GetRequiredService<TmeDbContextFactory>().CreateDbContext())
    {
      dbContext.Database.Migrate();
    }

    var navigationService = _host.Services.GetRequiredService<NavigationService<AcntTxnListingVM>>(); // initial naviation to the startup page.
    navigationService.Navigate();

    MainWindow = _host.Services.GetRequiredService<MainWindow>(); // new MainWindow() { DataContext = new MainVM(navigationStore) };
    MainWindow.Show();

    base.OnStartup(e);
  }

  protected override void OnExit(ExitEventArgs e)
  {
    _host.Dispose();
    base.OnExit(e);
  }

  //MakeAcntTxnVM CreateMakeAcntTxnVM() => new MakeAcntTxnVM(_bankAccountStore, new NavigationService(_navigationStore, CreateAcntTxnListingVM));
  ////tu: async viod / ctor:  ... ListingVM() => new AcntTxnListingVM          (_bankAccount, new NavigationService(_navigationStore, CreateMakeAcntTxnVM));
  //AcntTxnListingVM CreateAcntTxnListingVM() => AcntTxnListingVM.LoadViewModel(_bankAccountStore, new NavigationService(_navigationStore, CreateMakeAcntTxnVM));
}
