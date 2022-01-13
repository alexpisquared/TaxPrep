namespace TxnManualEntry2022.Commands;

public abstract class AsyncCommandBase : CommandBase
{
  bool _e; bool IsExecuting { get { return _e; } set { _e = value; OnCanExecuteChanged(); } }

  public override bool CanExecute(object? parameter) => !IsExecuting && base.CanExecute(parameter);

  public override async void Execute(object? parameter)
  {
    IsExecuting = true;
    try
    {
      await ExecuteAsync(parameter);
    }
    finally
    {
      IsExecuting = false;
    }
  }

  public abstract Task ExecuteAsync(object? parameter);
}