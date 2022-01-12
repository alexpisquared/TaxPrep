namespace TxnManualEntry2022.ViewModels;

internal class AcntTxnListingVM : ViewModelBase
{
  readonly ObservableCollection<AccountTxnVM> _txns;
  public AcntTxnListingVM()
  {
    _txns = new ObservableCollection<AccountTxnVM>();
  }

  internal IEnumerable<AccountTxnVM> Txns => _txns;
  public ICommand SubmitCommand { get; }

}
