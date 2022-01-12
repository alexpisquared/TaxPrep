namespace TxnManualEntry2022.ViewModels;

internal class AccountTxnVM : ViewModelBase
{
  readonly AccountTxn _accountTxn;

  public AccountTxnVM(AccountTxn accountTxn)
  {
    this._accountTxn = accountTxn;
  }
  public TxnTypeID TxnTypeID => _accountTxn.TxnTypeID;
  public DateTime TxnTime => _accountTxn.TxnTime;
  public decimal TxnAmount => _accountTxn.TxnAmount;
  public string UserID => _accountTxn.UserID;


}
