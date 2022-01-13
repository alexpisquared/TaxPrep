namespace TxnManualEntry2022;

public partial class App : Application
{
  readonly BankAccount _bankAccount;
  readonly NavigationStore _navigationStore;

  public App()
  {
    _bankAccount = new BankAccount("Good Account Name");
    _navigationStore = new NavigationStore();
  }

  protected override void OnStartup(StartupEventArgs e)
  {
    _navigationStore.CurrentModel = CreateAcntTxnListingVM();

    MainWindow = new MainWindow() { DataContext = new MainVM(_navigationStore) };
    MainWindow.Show();

    base.OnStartup(e);
  }

  MakeAcntTxnVM CreateMakeAcntTxnVM() => new MakeAcntTxnVM(_bankAccount, _navigationStore, CreateAcntTxnListingVM);

  AcntTxnListingVM CreateAcntTxnListingVM() => new AcntTxnListingVM(_navigationStore, CreateMakeAcntTxnVM);
}
