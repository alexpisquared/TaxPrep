namespace TxnManualEntry2022.ViewModels;

public class ViewModelBase : INotifyPropertyChanged
{
  public event PropertyChangedEventHandler? PropertyChanged;

  protected void OnPropertyChanged(string propName)
  {
    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
  }
}
