namespace TxnManualEntry2022.ViewModels;

public class MainVM : ViewModelBase
{
  readonly NavigationStore _navigationStore;

  public MainVM(NavigationStore navigationStore)
  {
    _navigationStore = navigationStore;
    _navigationStore.CurrentViewModelChanged += OnNavigationStore_CurrentViewModelChanged; ;
  }

  private void OnNavigationStore_CurrentViewModelChanged() => OnPropertyChanged(nameof(CurrentVM));

  public ViewModelBase CurrentVM => _navigationStore.CurrentModel;

  //protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
  //{
  //  if (!Equals(field, newValue))
  //  {
  //    field = newValue;
  //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
  //    return true;
  //  }

  //  return false;
  //}

  //object currentViewModel;  public object CurrentViewModel { get => currentViewModel; set => SetProperty(ref currentViewModel, value); }
}
