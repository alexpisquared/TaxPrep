namespace TxnManualEntry2022.ViewModels;

public class AcntTxnListingVM : ViewModelBase
{
  readonly ObservableCollection<AccountTxnVM> _txns;
  
  public IEnumerable<AccountTxnVM> Txns => _txns;

  public ICommand LoadAcntTxnsCommand { get; }
  public ICommand MakeAcntTxnCommand { get; }


  public AcntTxnListingVM(BankAccountStore bankAccountStore, NavigationService makeAcntTcnViewnavigationService)
  {
    _txns = new ObservableCollection<AccountTxnVM>();

    LoadAcntTxnsCommand = new LoadReservationCommand(this, bankAccountStore);
    MakeAcntTxnCommand = new NavigateCommand(makeAcntTcnViewnavigationService);

    AddTxnCommand = new NavigateCommand(makeAcntTcnViewnavigationService);
  }

  public static AcntTxnListingVM LoadViewModel(BankAccountStore bankAccountStore, NavigationService makeAcntTcnViewnavigationService) //tu: unloading ctors from work!!!!!!!!
  {
    AcntTxnListingVM viewModel = new AcntTxnListingVM(bankAccountStore, makeAcntTcnViewnavigationService);
    viewModel.LoadAcntTxnsCommand.Execute(null);
    return viewModel;
  }

 public void UpdateReservations(IEnumerable<AccountTxn> accountTxns)
  {
    _txns.Clear();
    foreach (var txn in accountTxns)
    {
      _txns.Add(new AccountTxnVM(txn));
    }
  }


  public ICommand AddTxnCommand { get; }
}
