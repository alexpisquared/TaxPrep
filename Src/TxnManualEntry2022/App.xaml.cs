using TxnManualEntry2022.Stores;

namespace TxnManualEntry2022;

public partial class App : Application
{
  const string ConnectionString = "Data Source=tme.db";
  //TmeDbContextFactory _tmeDbContextFactory;
  //readonly BankAccount _bankAccount;
  //readonly NavigationStore _navigationStore;
  //readonly BankAccountStore _bankAccountStore;
  IHost _host;

  public App()
  {
    _host = Host.CreateDefaultBuilder().ConfigureServices(services =>
    {
      services.AddSingleton<TmeDbContextFactory>(new TmeDbContextFactory(ConnectionString));          //_tmeDbContextFactory = new TmeDbContextFactory(ConnectionString);
      services.AddSingleton<IReservationProvider, DatabaseReservationProvider>();                     //IReservationProvider reservationProvider = new DatabaseReservationProvider(_tmeDbContextFactory);
      services.AddSingleton<IReservationCreator, DatabaseReservationCreator>();                       //IReservationCreator reservationCreator = new DatabaseReservationCreator(_tmeDbContextFactory);
      services.AddSingleton<IReservationConflictValidator, DatabaseReservationConflictValidator>();   //IReservationConflictValidator reservationConflictValidator = new DatabaseReservationConflictValidator(_tmeDbContextFactory); 
      services.AddTransient<TxnBook>();                                                               // TxnBook txnBook = new TxnBook(reservationProvider, reservationCreator, reservationConflictValidator);
      services.AddSingleton<BankAccount>((s) => new BankAccount("CIBC Viza", s.GetRequiredService<TxnBook>()));  //_bankAccount = new BankAccount("CIBC Viza", txnBook);
      services.AddSingleton<BankAccountStore>();                                                      //_bankAccountStore = new BankAccountStore(_bankAccount);
      services.AddSingleton<NavigationStore>();                                                       //_navigationStore = new NavigationStore();

      services.AddSingleton<MainVM>();
      services.AddSingleton(s => new MainWindow() { DataContext = s.GetRequiredService<MainVM>() });

    }).Build();
  }

  protected override void OnStartup(StartupEventArgs e)
  {
    _host.Start();

    //DbContextOptions options = new DbContextOptionsBuilder().UseSqlite(ConnectionString).Options;
    using (TmeDbContext dbContext = _host.Services.GetRequiredService<TmeDbContextFactory>().CreateDbContext())
    {
      dbContext.Database.Migrate();
    }

    var navigationStore = _host.Services.GetRequiredService<NavigationStore>();
    navigationStore.CurrentModel = CreateAcntTxnListingVM();

    MainWindow = _host.Services.GetRequiredService<MainWindow>(); // new MainWindow() { DataContext = new MainVM(navigationStore) };
    MainWindow.Show();

    base.OnStartup(e);
  }

  MakeAcntTxnVM CreateMakeAcntTxnVM() => new MakeAcntTxnVM(_bankAccountStore, new NavigationService(_navigationStore, CreateAcntTxnListingVM));


  //tu: async viod / ctor:  ... ListingVM() => new AcntTxnListingVM          (_bankAccount, new NavigationService(_navigationStore, CreateMakeAcntTxnVM));
  AcntTxnListingVM CreateAcntTxnListingVM() => AcntTxnListingVM.LoadViewModel(_bankAccountStore, new NavigationService(_navigationStore, CreateMakeAcntTxnVM));
}
