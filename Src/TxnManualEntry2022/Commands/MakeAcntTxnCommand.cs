namespace TxnManualEntry2022.Commands;

public class MakeAcntTxnCommand : AsyncCommandBase
{
  readonly BankAccountStore _bankAccountStore;
  readonly MakeAcntTxnVM _makeAcntTxnVM;
  readonly NavigationService<AcntTxnListingVM> _makeAcntTcnViewnavigationService;

  public MakeAcntTxnCommand(MakeAcntTxnVM makeAcntTxnVM, BankAccountStore bankAccountStore, NavigationService<AcntTxnListingVM> makeAcntTcnViewnavigationService)
  {
    _makeAcntTxnVM = makeAcntTxnVM;
    _bankAccountStore = bankAccountStore;
    _makeAcntTcnViewnavigationService = makeAcntTcnViewnavigationService;
    _makeAcntTxnVM.PropertyChanged += OnMakeAcntTxnVM_PropertyChanged;
  }

  private void OnMakeAcntTxnVM_PropertyChanged(object? sender, PropertyChangedEventArgs e)
  {
    if (
      e.PropertyName == nameof(MakeAcntTxnVM.TxnAmnt) ||
      e.PropertyName == nameof(MakeAcntTxnVM.TxnTime))
      OnCanExecuteChanged();
  }

  public override bool CanExecute(object? parameter) =>
    _makeAcntTxnVM.TxnAmnt != 0 &&
    _makeAcntTxnVM.TxnTime > DateTime.MinValue &&
    base.CanExecute(parameter);

  public override async Task ExecuteAsync(object? parameter)
  {
    var accountTxn = new AccountTxn(
      new TxnTypeID(_makeAcntTxnVM.ID, _makeAcntTxnVM.Name), _makeAcntTxnVM.TxnTime, _makeAcntTxnVM.TxnAmnt.ToString());

    try
    {
      await _bankAccountStore.MakeReservation(accountTxn);

      MessageBox.Show("Success adding Txn");

      _makeAcntTcnViewnavigationService.Navigate();
    }
    catch (TxnConflictException) { WriteLine($"!!> Txn already there "); if (Debugger.IsAttached) Debugger.Break(); MessageBox.Show($"!!> Txn already there "); }
    catch (Exception ex) { WriteLine($"!!> {ex}"); if (Debugger.IsAttached) Debugger.Break(); MessageBox.Show(ex.Message, "Failed to add Txn"); }
  }
}
