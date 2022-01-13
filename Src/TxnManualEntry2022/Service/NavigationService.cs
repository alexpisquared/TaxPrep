namespace TxnManualEntry2022.Service;

public class NavigationService<TViewModel> where TViewModel : ViewModelBase
{
  readonly NavigationStore _navigationStore;
  readonly Func<TViewModel> _createVM;

  public NavigationService(NavigationStore navigationStore, Func<TViewModel> createVM)
  {
    _navigationStore = navigationStore;
    _createVM = createVM;
  }

  public void Navigate()
  {
    _navigationStore.CurrentModel = _createVM();
  }
}
