using TxnManualEntry2022.Models;

namespace TxnManualEntry2022.Stores;

public class BankAccountStore
{
  readonly BankAccount _bankAccount;
  readonly List<AccountTxn> _accountTxns;
  public IEnumerable<AccountTxn> AccountTxns => _accountTxns;

  public BankAccountStore(BankAccount bankAccount)
  {
    _bankAccount = bankAccount; 
    _accountTxns = new List<AccountTxn>();
  }

  public async Task Load()
{
    IEnumerable<AccountTxn> accountTxns = await  _bankAccount.GetAllTxnx();

    _accountTxns.Clear(); 
    _accountTxns.AddRange(accountTxns);
  }

}
