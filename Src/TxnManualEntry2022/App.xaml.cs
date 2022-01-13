namespace TxnManualEntry2022;

public partial class App : Application
{
  private const string ConnectionString = "Data Source=tme.db";
  readonly BankAccount _bankAccount;
  readonly NavigationStore _navigationStore;
  readonly BankAccountStore _bankAccountStore;
  TmeDbContextFactory _tmeDbContextFactory;

  public App()
  {
    _tmeDbContextFactory = new TmeDbContextFactory(ConnectionString);
    IReservationProvider reservationProvider = new DatabaseReservationProvider(_tmeDbContextFactory);
    IReservationCreator reservationCreator = new DatabaseReservationCreator(_tmeDbContextFactory);
    IReservationConflictValidator reservationConflictValidator = new  DatabaseReservationConflictValidator(_tmeDbContextFactory);

    TxnBook txnBook = new TxnBook(reservationProvider, reservationCreator, reservationConflictValidator);
    _bankAccount = new BankAccount("Good Account Name", txnBook);
    _bankAccountStore = new BankAccountStore(_bankAccount);
    _navigationStore = new NavigationStore();
  }

  protected override void OnStartup(StartupEventArgs e)
  {
    //DbContextOptions options = new DbContextOptionsBuilder().UseSqlite(ConnectionString).Options;
    using (TmeDbContext dbContext = _tmeDbContextFactory.CreateDbContext())
    {
      dbContext.Database.Migrate();
    }

    _navigationStore.CurrentModel = CreateAcntTxnListingVM();

    MainWindow = new MainWindow() { DataContext = new MainVM(_navigationStore) };
    MainWindow.Show();

    base.OnStartup(e);
  }

  MakeAcntTxnVM CreateMakeAcntTxnVM() => new MakeAcntTxnVM(_bankAccountStore, new NavigationService(_navigationStore, CreateAcntTxnListingVM));

  
  //tu: async viod / ctor:  ... ListingVM() => new AcntTxnListingVM          (_bankAccount, new NavigationService(_navigationStore, CreateMakeAcntTxnVM));
  AcntTxnListingVM CreateAcntTxnListingVM() => AcntTxnListingVM.LoadViewModel(_bankAccountStore, new NavigationService(_navigationStore, CreateMakeAcntTxnVM));
}
