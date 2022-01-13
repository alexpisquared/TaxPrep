namespace TxnManualEntry2022.ViewModels;

public class AcntTxnListingVM : ViewModelBase
{
  readonly ObservableCollection<AccountTxnVM> _txns;
  readonly BankAccount _bankAccount;

  public AcntTxnListingVM(BankAccount bankAccount, NavigationService makeAcntTcnViewnavigationService)
  {
    _txns = new ObservableCollection<AccountTxnVM>();

    AddTxnCommand = new NavigateCommand(makeAcntTcnViewnavigationService);

    _bankAccount = bankAccount;
    UpdateReservations();
  }

  void UpdateReservations()
  {
    _txns.Clear();
    foreach (var txn in _bankAccount.GetAllTxnx())
    {
      _txns.Add(new AccountTxnVM(txn));
    }
  }

  public ObservableCollection<AccountTxnVM> Txns => _txns;

  public ICommand AddTxnCommand { get; }
}
