namespace MinFin7b;
public partial class MainWindow : Window
{
  readonly FinDemoContext _db = new();
  public MainWindow() => InitializeComponent();

  void OnLoaded(object sender, RoutedEventArgs e)
  {
    try
    {
      Trace.WriteLine($"{_db.TxCategories.Count()}");
      Trace.WriteLine($"{_db.TxCategories.Count()}   {_db.TxCategories.Local.Count()}   ");

      _db.TxCategories.Load();

      Trace.WriteLine($"{_db.TxCategories.Count()}   {_db.TxCategories.Local.Count()}   ");
      dg1.ItemsSource = _db.TxCategories.Local.Take(3);

    }
    catch (Exception ex)
    {
      Trace.WriteLine($"{ex}");
      throw;
    }
  }

  void OnClose(object sender, RoutedEventArgs e) => Close();
}
