namespace TxnManualEntry2022.ViewModels;

public class AcntTxnListingVM : ViewModelBase
{
  readonly ObservableCollection<AccountTxnVM> _txns;
  
  public IEnumerable<AccountTxnVM> Txns => _txns;

  public ICommand LoadAcntTxnsCommand { get; }
  public ICommand MakeAcntTxnCommand { get; }


  public AcntTxnListingVM(BankAccount bankAccount, NavigationService makeAcntTcnViewnavigationService)
  {
    _txns = new ObservableCollection<AccountTxnVM>();

    LoadAcntTxnsCommand = new LoadReservationCommand(this, bankAccount);
    MakeAcntTxnCommand = new NavigateCommand(makeAcntTcnViewnavigationService);

    //AddTxnCommand = new NavigateCommand(makeAcntTcnViewnavigationService);
    //    UpdateReservations();
  }

  public static AcntTxnListingVM LoadViewModel(BankAccount bankAccount, NavigationService makeAcntTcnViewnavigationService) //tu: unloading ctors from work!!!!!!!!
  {
    AcntTxnListingVM viewModel = new AcntTxnListingVM(bankAccount, makeAcntTcnViewnavigationService);
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
