﻿namespace TxnManualEntry2022.ViewModels;

public class AcntTxnListingVM : ViewModelBase
{
  readonly BankAccountStore _bankAccountStore;
  readonly ObservableCollection<AccountTxnVM> _accountTxnVMs;

  public IEnumerable<AccountTxnVM> AccountTxnVMs => _accountTxnVMs;

  private string _e;

  public string ErrorMessage
  {
    get { return _e; }
    set
    {
      _e = value;
      OnPropertyChanged(nameof(ErrorMessage));
      OnPropertyChanged(nameof(HasErrorMessage));
    }
  }
  public bool HasErrorMessage => !string.IsNullOrEmpty(ErrorMessage);

  bool _l; public bool IsLoading { get { return _l; } set { _l = value; OnPropertyChanged(nameof(IsLoading)); } }

  public ICommand LoadAcntTxnsCommand { get; }
  public ICommand MakeAcntTxnCommand { get; }

  public AcntTxnListingVM(BankAccountStore bankAccountStore, NavigationService<MakeAcntTxnVM> makeAcntTcnViewnavigationService)
  {
    _bankAccountStore = bankAccountStore;
    _accountTxnVMs = new ObservableCollection<AccountTxnVM>();

    LoadAcntTxnsCommand = new LoadReservationCommand(this, bankAccountStore);
    MakeAcntTxnCommand = new NavigateCommand<MakeAcntTxnVM>(makeAcntTcnViewnavigationService);

    AddTxnCommand = new NavigateCommand<MakeAcntTxnVM>(makeAcntTcnViewnavigationService); //todo: ?? not in the org: https://youtu.be/ZQadPrZ7E_A?list=PLA8ZIAm2I03hS41Fy4vFpRw8AdYNBXmNm&t=482

    bankAccountStore.AccountTxnsMade += OnAccountTxnsMade;
  }

  ~AcntTxnListingVM() => WriteLine($"@@@:> ~dtor called!");

  public override void Dispose()
  {
    _bankAccountStore.AccountTxnsMade -= OnAccountTxnsMade;
    base.Dispose();
  }

  private void OnAccountTxnsMade(AccountTxn actnTxn)
  {
    var accountTxnVM = new AccountTxnVM(actnTxn);
    _accountTxnVMs.Add(accountTxnVM);

  }

  public static AcntTxnListingVM LoadViewModel(BankAccountStore bankAccountStore, NavigationService<MakeAcntTxnVM> makeAcntTcnViewnavigationService) //tu: unloading ctors from work!!!!!!!!
  {
    var viewModel = new AcntTxnListingVM(bankAccountStore, makeAcntTcnViewnavigationService);
    viewModel.LoadAcntTxnsCommand.Execute(null);
    return viewModel;
  }

  public void UpdateReservations(IEnumerable<AccountTxn> accountTxns)
  {
    _accountTxnVMs.Clear();
    foreach (var txn in accountTxns)
    {
      _accountTxnVMs.Add(new AccountTxnVM(txn));
    }
  }


  public ICommand AddTxnCommand { get; }
}
