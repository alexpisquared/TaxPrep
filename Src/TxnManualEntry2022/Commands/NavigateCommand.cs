namespace TxnManualEntry2022.Commands
{
  internal class NavigateCommand : CommandBase
  {
    readonly NavigationStore _navigationStore;
    private readonly Func<ViewModelBase> _createVM;

    public NavigateCommand(NavigationStore navigationStore, Func<ViewModelBase> createVM)
    {
      _navigationStore = navigationStore;
      _createVM = createVM;
    }

    public override void Execute(object? parameter) =>
      _navigationStore.CurrentModel = _createVM();
  }
}
