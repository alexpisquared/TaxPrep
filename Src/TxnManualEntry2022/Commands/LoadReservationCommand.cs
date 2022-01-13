using System.Collections.Generic;

namespace TxnManualEntry2022.Commands;

public class LoadReservationCommand : AsyncCommandBase //tu: async void (#2)
{
  readonly BankAccountStore _bankAccountStore;
  readonly AcntTxnListingVM _vm;

  public LoadReservationCommand( AcntTxnListingVM vm, BankAccountStore bankAccountStore)
  {
    _bankAccountStore = bankAccountStore;
    _vm = vm;
  }

  public override async Task ExecuteAsync(object? parameter)
  {
    try
    {
      await _bankAccountStore.Load();
      _vm.UpdateReservations(_bankAccountStore.AccountTxns);
    }
    catch (Exception ex) { WriteLine($"!!> {ex}"); if (Debugger.IsAttached) Debugger.Break(); MessageBox.Show(ex.Message, "Failed to load Txns"); }
  }
}
