namespace TxnManualEntry2022.Models;

public class BankAccount
{
 readonly TxnBook _txnBook;
  public string Name { get; }

  public BankAccount(string name, TxnBook txnBook)
  {
    Name = name;
    _txnBook = txnBook;
  }

  public async Task< IEnumerable<AccountTxn>> GetAllTxnx() => await _txnBook.GetAllTxnx();

  public async Task AddTxn(AccountTxn accountTxn) => await _txnBook.AddTxn(accountTxn);

}
