﻿using System.Collections.Generic;

namespace TxnManualEntry2022.Commands;

public class LoadReservationCommand : AsyncCommandBase //tu: async void (#2)
{
  readonly BankAccount _bankAccount;
  readonly AcntTxnListingVM _vm;

  public LoadReservationCommand( AcntTxnListingVM vm, BankAccount bankAccount)
  {
    _bankAccount = bankAccount;
    _vm = vm;
  }

  public override async Task ExecuteAsync(object? parameter)
  {
    try
    {
      IEnumerable<AccountTxn> accountTxns = await _bankAccount.GetAllTxnx();
      _vm.UpdateReservations(accountTxns);
    }
    catch (Exception ex) { WriteLine($"!!> {ex}"); if (Debugger.IsAttached) Debugger.Break(); MessageBox.Show(ex.Message, "Failed to load Txns"); }
  }
}