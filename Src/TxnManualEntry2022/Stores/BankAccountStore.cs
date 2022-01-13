using TxnManualEntry2022.Models;

namespace TxnManualEntry2022.Stores;

public class BankAccountStore
{
  readonly BankAccount _bankAccount;
  readonly List<AccountTxn> _accountTxns;
  Lazy<Task> _initializeLazy; //tu: runs once only 

  public IEnumerable<AccountTxn> AccountTxns => _accountTxns;

  public event Action<AccountTxn> AccountTxnsMade;

  public BankAccountStore(BankAccount bankAccount)
  {
    _bankAccount = bankAccount;
    _initializeLazy = new Lazy<Task>(Initialize);
    _accountTxns = new List<AccountTxn>();
  }

  public async Task Load()
  {
    try
    {
      await _initializeLazy.Value; //tu: runs once only 
    }
    catch (Exception)
    {
      _initializeLazy = new Lazy<Task>(Initialize);
      throw;
    }
  }

  public async Task MakeReservation(AccountTxn accountTxn)
  {
    await _bankAccount.AddTxn(accountTxn);

    _accountTxns.Add(accountTxn);

    OnAccountTxnMade(accountTxn);
  }

  private void OnAccountTxnMade(AccountTxn accountTxn)
  {
    AccountTxnsMade?.Invoke(accountTxn);
  }

  private async Task Initialize()
  {
    IEnumerable<AccountTxn> accountTxns = await _bankAccount.GetAllTxnx();

    _accountTxns.Clear();
    _accountTxns.AddRange(accountTxns);

    throw new Exception("@@@@@@@@@@@ Testing Testing Testing Testing Testing @@@@@@@@@@@@");
  }
}
