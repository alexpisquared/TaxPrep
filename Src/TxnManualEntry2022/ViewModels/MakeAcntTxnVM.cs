namespace TxnManualEntry2022.ViewModels;

internal class MakeAcntTxnVM : ViewModelBase
{
  decimal txnAmnt;
  DateTime txnTime;

  public MakeAcntTxnVM()
  {

  }

  public decimal TxnAmnt { get => txnAmnt; set { txnAmnt = value; OnPropertyChanged(nameof(TxnAmnt)); } }
  public DateTime TxnTime { get => txnTime; set { txnTime = value; OnPropertyChanged(nameof(TxnTime)); } }

  public ICommand SubmitCommand { get; }
  public ICommand CancelCommand { get; }
}
