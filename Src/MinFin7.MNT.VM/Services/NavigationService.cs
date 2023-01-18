using MinFin7.MNT.Stores;
using MinFin7.MNT.VM.Services;
using MinFin7.MNT.VM.VMs;

namespace MinFin7.MNT.Services;

public class NavSvc<TVM> : INavSvc where TVM : BaseMinVM
{
  readonly NavigationStore _navigationStore;
  readonly Func<TVM> _createVM;

  public NavSvc(NavigationStore navigationStore, Func<TVM> createVM)
  {
    _navigationStore = navigationStore;
    _createVM = createVM;
  }

  public void Navigate() => _navigationStore.CurrentVM = _createVM();
}