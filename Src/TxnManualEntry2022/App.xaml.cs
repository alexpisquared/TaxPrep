using TxnManualEntry2022.ViewModels;

namespace TxnManualEntry2022;

public partial class App : Application
{
  const string UserID = "UserID is not used";
  readonly BankAccount _bankAccount;
  public App()
  {
    _bankAccount = new BankAccount("Good Account Name");
  }

  protected override void OnStartup(StartupEventArgs e)
  {
    //var acnt = new BankAccount("CIBC VISA");
    //try
    //{
    //  acnt.AddTxn(new AccountTxn(new TxnTypeID(1, "Gifts"), new DateTime(2000, 1, 1), UserID));
    //  acnt.AddTxn(new AccountTxn(new TxnTypeID(2, "Foods"), new DateTime(2000, 2, 2), UserID));
    //  //acnt.AddTxn(new AccountTxn(new TxnTypeID(2, "Foods"), new DateTime(2000, 2, 2), UserID));
    //}
    //catch (TxnConflictException ex) { WriteLine($"!!> {ex}"); if (Debugger.IsAttached) Debugger.Break(); throw; }
    //catch (Exception ex) { WriteLine($"!!> {ex}"); if (Debugger.IsAttached) Debugger.Break(); throw; }
    //    var resultTxns = acnt.GetTxnxForUser(UserID);

    MainWindow = new MainWindow() { DataContext = new MainVM(_bankAccount) };
    MainWindow.Show();  

    base.OnStartup(e);
  }
}
