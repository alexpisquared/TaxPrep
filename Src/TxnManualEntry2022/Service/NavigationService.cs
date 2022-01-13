namespace TxnManualEntry2022.Service;

public class NavigationService
{
  readonly NavigationStore _navigationStore;
  readonly Func<ViewModelBase> _createVM;

  public NavigationService(NavigationStore navigationStore, Func<ViewModelBase> createVM)
  {
    _navigationStore = navigationStore;
    _createVM = createVM;
  }

  public void Navigate()
  {
    _navigationStore.CurrentModel = _createVM();
  }
}
