using System;
using TxnManualEntry2022.Commands;
using TxnManualEntry2022.Service;

namespace TxnManualEntry2022.ViewModels;

public class MakeAcntTxnVM : ViewModelBase
{
  readonly BankAccount _bankAccount;

  decimal txnAmnt;
  DateTime txnTime;

  public MakeAcntTxnVM(BankAccount bankAccount, NavigationService makeAcntTcnViewnavigationService)
  {
    _bankAccount = bankAccount;

    SubmitCommand = new MakeAcntTxnCommand(this, _bankAccount, makeAcntTcnViewnavigationService);
    CancelCommand = new NavigateCommand(makeAcntTcnViewnavigationService);

    TxnAmnt = 123.45m;
    TxnTime = DateTime.Now;
  }

  public decimal TxnAmnt { get => txnAmnt; set { txnAmnt = value; OnPropertyChanged(nameof(TxnAmnt)); } }
  public DateTime TxnTime { get => txnTime; set { txnTime = value; OnPropertyChanged(nameof(TxnTime)); } }
  public string Name { get; set; }
  public int ID { get; set; }

  public ICommand SubmitCommand { get; }
  public ICommand CancelCommand { get; }
}
