using System;
using TxnManualEntry2022.Commands;
using TxnManualEntry2022.Service;

namespace TxnManualEntry2022.ViewModels;

public class MakeAcntTxnVM : ViewModelBase
{
  //readonly BankAccountStore _bankAccountStore;

  decimal txnAmnt;
  DateTime txnTime;

  public MakeAcntTxnVM(BankAccountStore bankAccountStore, NavigationService makeAcntTcnViewnavigationService)
  {
    //_bankAccountStore = bankAccountStore;

    SubmitCommand = new MakeAcntTxnCommand(this, bankAccountStore, makeAcntTcnViewnavigationService);
    CancelCommand = new NavigateCommand(makeAcntTcnViewnavigationService);

    TxnAmnt = DateTime.Now.Hour + .01m * DateTime.Now.Minute;
    TxnTime = DateTime.Today;
  }

  public decimal TxnAmnt { get => txnAmnt; set { txnAmnt = value; OnPropertyChanged(nameof(TxnAmnt)); } }
  public DateTime TxnTime { get => txnTime; set { txnTime = value; OnPropertyChanged(nameof(TxnTime)); } }
  public string Name { get; set; }
  public int ID { get; set; }

  public ICommand SubmitCommand { get; }
  public ICommand CancelCommand { get; }
}
