namespace TxnManualEntry2022.ViewModels;

public class MakeAcntTxnVM : ViewModelBase, INotifyDataErrorInfo
{
  //readonly BankAccountStore _bankAccountStore;

  decimal txnAmnt;
  DateTime txnTime;

  public MakeAcntTxnVM(BankAccountStore bankAccountStore, NavigationService makeAcntTcnViewnavigationService)
  {
    //_bankAccountStore = bankAccountStore;

    SubmitCommand = new MakeAcntTxnCommand(this, bankAccountStore, makeAcntTcnViewnavigationService);
    CancelCommand = new NavigateCommand(makeAcntTcnViewnavigationService);

    TxnAmnt = DateTime.Now.Hour + .01m * DateTime.Now.Minute;
    TxnTime = DateTime.Today;
  }

  public decimal TxnAmnt
  {
    get => txnAmnt; set
    {
      txnAmnt = value; 
      OnPropertyChanged(nameof(TxnAmnt));

      OnPropertyChanged(nameof(TxnTime)); // a demo for interdependant values (like StartDate/EndDate for example). 
      ClearErrors(nameof(TxnAmnt));
      if (TxnAmnt < 0)
      {
        AddError("The txn amount cannot be in the future.", nameof(TxnAmnt));
      }
    }
  }
  public DateTime TxnTime
  {
    get => txnTime; set
    {
      txnTime = value;
      OnPropertyChanged(nameof(TxnTime));

      ClearErrors(nameof(TxnTime));
      if (TxnTime > DateTime.Now)
      {
        AddError("The txn time cannot be in the future.", nameof(TxnTime));
      }
    }
  }

  private void AddError(string errorMessage, string propName)
  {
    if (!_propNameToErrorsDictionary.ContainsKey(propName))
      _propNameToErrorsDictionary.Add(propName, new List<string>());

    _propNameToErrorsDictionary[propName].Add(errorMessage);

    OnErrorsChanged(propName);
  }

  private void OnErrorsChanged(string propName) => ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propName));
  private void ClearErrors(string propName)
  {
    _propNameToErrorsDictionary.Remove(propName);
    OnErrorsChanged(propName);
  }

  public string Name { get; set; }

  public int ID { get; set; }

  public ICommand SubmitCommand { get; }
  public ICommand CancelCommand { get; }

  readonly Dictionary<string, List<string>> _propNameToErrorsDictionary = new Dictionary<string, List<string>>();

  public bool HasErrors => _propNameToErrorsDictionary.Any();

  public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

  public IEnumerable GetErrors(string? propertyName) => _propNameToErrorsDictionary.GetValueOrDefault(propertyName, new List<string>());
}
