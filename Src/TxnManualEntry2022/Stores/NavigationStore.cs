namespace TxnManualEntry2022.Stores
{
  internal class NavigationStore
  {
    private ViewModelBase _currentModel;

    public ViewModelBase CurrentModel
    {
      get => _currentModel; set
      {
        _currentModel = value;
        OnCurrentViewModelChanged();
      }
    }

    void OnCurrentViewModelChanged() => CurrentViewModelChanged?.Invoke();

    public event Action CurrentViewModelChanged;
  }
}
