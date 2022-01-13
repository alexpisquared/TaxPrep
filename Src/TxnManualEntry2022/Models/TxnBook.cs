using TxnManualEntry2022.Exceptions;

namespace TxnManualEntry2022.Models;

internal class TxnBook // res. book
{
  readonly List<AccountTxn> _accountTxns = new List<AccountTxn>();

  public IEnumerable<AccountTxn> GetAllTxnx() => _accountTxns.ToList();
  public IEnumerable<AccountTxn> GetTxnxForUser(string userid) => _accountTxns.Where(r => r.UserID.Equals(userid)).ToList();

  public void AddTxn(AccountTxn accountTxn)
  {
    foreach (var existingTxn in _accountTxns)
    {
      if (existingTxn.Conflicts(accountTxn))
      {
        throw new TxnConflictException(existingTxn, accountTxn);
      }
    }
    try
    {
      _accountTxns.Add(accountTxn);
    }
    catch (Exception ex) {
    MessageBox.Show(ex.Message);
    }
  }
}
