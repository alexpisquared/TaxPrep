using System.Collections.Generic;

namespace TxnManualEntry2022.Commands;

public class LoadReservationCommand : AsyncCommandBase //tu: async void (#2)
{
  readonly BankAccountStore _bankAccountStore;
  readonly AcntTxnListingVM _vm;

  public LoadReservationCommand(AcntTxnListingVM vm, BankAccountStore bankAccountStore)
  {
    _bankAccountStore = bankAccountStore;
    _vm = vm;
  }

  public override async Task ExecuteAsync(object? parameter)
  {
    _vm.IsLoading = true;
    _vm.ErrorMessage = "";
    try
    {
      await _bankAccountStore.Load();

      //throw new Exception("@@@@@@@@@@@ Testing Testing Testing Testing Testing @@@@@@@@@@@@");
      
      _vm.UpdateReservations(_bankAccountStore.AccountTxns);
    }
    catch (Exception ex)
    {
      WriteLine($"!!> {ex}"); if (Debugger.IsAttached) Debugger.Break();
      _vm.ErrorMessage = $"Failed to load Txns   {ex.Message}";
    }

    _vm.IsLoading = false;
  }
}
