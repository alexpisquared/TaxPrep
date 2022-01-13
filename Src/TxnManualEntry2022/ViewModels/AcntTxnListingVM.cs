using TxnManualEntry2022.Commands;

namespace TxnManualEntry2022.ViewModels;

internal class AcntTxnListingVM : ViewModelBase
{
  readonly ObservableCollection<AccountTxnVM> _txns;

  public AcntTxnListingVM()
  {
    _txns = new ObservableCollection<AccountTxnVM>();

    AddTxnCommand = new NavigateCommand();

    _txns.Add(new AccountTxnVM(new AccountTxn(new TxnTypeID(1, "Auto"), DateTime.Now, "Not used 1.")));
    _txns.Add(new AccountTxnVM(new AccountTxn(new TxnTypeID(2, "Food"), DateTime.Now, "Not used 2.")));
  }

  internal ObservableCollection<AccountTxnVM> Txns => _txns;

  public ICommand AddTxnCommand { get; }
}
