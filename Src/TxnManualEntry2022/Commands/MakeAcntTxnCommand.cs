using TxnManualEntry2022.ViewModels;

namespace TxnManualEntry2022.Commands
{
  internal class MakeAcntTxnCommand : CommandBase
  {
    readonly BankAccount _bankAccount;
    readonly MakeAcntTxnVM _makeAcntTxnVM;

    public MakeAcntTxnCommand(MakeAcntTxnVM makeAcntTxnVM, BankAccount bankAccount)
    {
      _makeAcntTxnVM = makeAcntTxnVM;
      _bankAccount = bankAccount;

      _makeAcntTxnVM.PropertyChanged += OnMakeAcntTxnVM_PropertyChanged;
    }

    private void OnMakeAcntTxnVM_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
      if(
        e.PropertyName == nameof(MakeAcntTxnVM.TxnAmnt) ||
        e.PropertyName == nameof(MakeAcntTxnVM.TxnTime)        )
        OnCanExecuteChanged();
    }

    public override bool CanExecute(object? parameter) => 
      _makeAcntTxnVM.TxnAmnt != 0 &&
      _makeAcntTxnVM.TxnTime > DateTime.MinValue && 
      base.CanExecute(parameter);

    public override void Execute(object? parameter)
    {
      var accountTxn = new AccountTxn(
        new TxnTypeID(_makeAcntTxnVM.ID, _makeAcntTxnVM.Name), _makeAcntTxnVM.TxnTime, _makeAcntTxnVM.TxnAmnt.ToString());

      try
      {
        _bankAccount.AddTxn(accountTxn);
        MessageBox.Show("Success adding Txn");
      }
      catch (TxnConflictException) { WriteLine($"!!> Txn already there "); if (Debugger.IsAttached) Debugger.Break(); MessageBox.Show($"!!> Txn already there "); }
      catch (Exception ex) { WriteLine($"!!> {ex}"); if (Debugger.IsAttached) Debugger.Break(); MessageBox.Show(ex.Message); }
    }
  }
}
