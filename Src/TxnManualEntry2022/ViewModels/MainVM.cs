namespace TxnManualEntry2022.ViewModels;

internal class MainVM : ViewModelBase
{
  public MainVM(BankAccount bankAccount)
  {
    CurrentVM = new
      AcntTxnListingVM();
    //MakeAcntTxnVM      (bankAccount);
  }

  public ViewModelBase CurrentVM { get; }
}
