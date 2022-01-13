using System;
using TxnManualEntry2022.Commands;

namespace TxnManualEntry2022.ViewModels;

internal class MakeAcntTxnVM : ViewModelBase
{
  readonly BankAccount _bankAccount;

  decimal txnAmnt;
  DateTime txnTime;

  public MakeAcntTxnVM(BankAccount bankAccount, NavigationStore navigationStore, Func<AcntTxnListingVM> createVAcntTxnListingVM)
  {
    _bankAccount = bankAccount;

    SubmitCommand = new MakeAcntTxnCommand(this, _bankAccount);
    CancelCommand = new NavigateCommand(navigationStore, createVAcntTxnListingVM);

    TxnAmnt = 123.45m;
    TxnTime = DateTime.Now;
  }

  public decimal TxnAmnt { get => txnAmnt; set { txnAmnt = value; OnPropertyChanged(nameof(TxnAmnt)); } }
  public DateTime TxnTime { get => txnTime; set { txnTime = value; OnPropertyChanged(nameof(TxnTime)); } }
  public string Name { get; internal set; }
  public int ID { get; internal set; }

  public ICommand SubmitCommand { get; }
  public ICommand CancelCommand { get; }
}
