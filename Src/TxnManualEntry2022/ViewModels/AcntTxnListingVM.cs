namespace TxnManualEntry2022.ViewModels;

internal class AcntTxnListingVM : ViewModelBase
{
  readonly ObservableCollection<AccountTxnVM> _txns;
  public AcntTxnListingVM()
  {
    _txns = new ObservableCollection<AccountTxnVM>();

    _txns.Add(new AccountTxnVM(new AccountTxn(new TxnTypeID(1, "Auto"), DateTime.Now, "Not used.")));
    _txns.Add(new AccountTxnVM(new AccountTxn(new TxnTypeID(2, "Food"), DateTime.Now, "Not used.")));
  }

  internal IEnumerable<AccountTxnVM> Txns => _txns;
  public ICommand SubmitCommand { get; }

}
