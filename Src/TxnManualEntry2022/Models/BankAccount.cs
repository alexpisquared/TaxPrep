namespace TxnManualEntry2022.Models;

internal class BankAccount
{
  TxnBook _txnBook;
  public string Name { get; }

  public BankAccount(string name)
  {
    Name = name;
    _txnBook = new TxnBook();
  }

  public IEnumerable<AccountTxn> GetTxnxForUser(string userid) => _txnBook. GetTxnxForUser( userid) ;

  public void AddTxn(AccountTxn accountTxn) => _txnBook.AddTxn(accountTxn);

}
