using TxnManualEntry2022.Models;

namespace TxnManualEntry2022.Stores;

public class BankAccountStore
{
  readonly BankAccount _bankAccount;
  readonly Lazy<Task> _initializeLazy; //tu: runs once only 
  readonly List<AccountTxn> _accountTxns;

  public IEnumerable<AccountTxn> AccountTxns => _accountTxns;

  public event Action <AccountTxn> AccountTxnsMade;

  public BankAccountStore(BankAccount bankAccount)
  {
    _bankAccount = bankAccount;
    _initializeLazy = new Lazy<Task>(Initialize);
    _accountTxns = new List<AccountTxn>();
  }

  public async Task Load()
  {
   await _initializeLazy.Value; //tu: runs once only 
  }

  public async Task MakeReservation(AccountTxn accountTxn)
{
    await _bankAccount.AddTxn(accountTxn);

    _accountTxns.Add(accountTxn);

    OnAccountTxnMade(accountTxn);
  }

  private void OnAccountTxnMade(AccountTxn accountTxn )
{
    AccountTxnsMade?.Invoke(accountTxn);
  }

  private async Task Initialize()
  {
    IEnumerable<AccountTxn> accountTxns = await _bankAccount.GetAllTxnx();

    _accountTxns.Clear();
    _accountTxns.AddRange(accountTxns);
  }
}
