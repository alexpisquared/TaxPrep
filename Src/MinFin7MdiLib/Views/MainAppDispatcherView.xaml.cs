﻿namespace MinFin7MdiLib.Views;
public partial class MainAppDispatcherView : UserControl
{
  SpeechSynth? _sth;
  ILogger? _lgr;
  IBpr? _bpr;

  public MainAppDispatcherView() { InitializeComponent(); }
  void OnLoaded(object sender, RoutedEventArgs e)
  {
    _lgr = ((BaseDbVM)DataContext).Lgr;
    _bpr = ((BaseDbVM)DataContext).Bpr;
    _sth = ((BaseDbVM)DataContext).Sth;
  }

  void SetDefault(int cb) { b1.IsDefault = b2.IsDefault = b3.IsDefault = b4.IsDefault = b5.IsDefault = b6.IsDefault = b7.IsDefault = b8.IsDefault = false; ((Button)FindName($"b{cb + 1}")).IsDefault = true; }
  async void OnX(object s, RoutedEventArgs e)
  {
    ArgumentNullException.ThrowIfNull(_lgr, nameof(_lgr));
    ArgumentNullException.ThrowIfNull(_bpr, nameof(_bpr));
    ArgumentNullException.ThrowIfNull(_sth, nameof(_sth));

    ((Button)s).IsEnabled = false;
    switch (((Button)s).Name)
    {
      //case "b1": setDefault(0); new DbLoaderReportWindow(MSMoneyDbLoader.App.GetCmndLineArgsInclClickOnce()).ShowDialog(); break;
      //case "b2": setDefault(1); new HistoricalChartSet.MainHistChart().Show(); break;
      case "b3": SetDefault(2); new TxCategoryAssignerVw(_lgr, _bpr, _sth).Show(); break;
      case "b4": SetDefault(3); new ManualTxnEntry(_lgr, _bpr, _sth, false).Show(); break;
      //case "b5": setDefault(4); MinFin7MdiLib.Report.WinForm.Program.ShowBoth(); break;
      //case "b6": setDefault(5); MinFin7MdiLib.Report.WinForm.Program.Show_Alx(); break;
      //case "b7": setDefault(6); MinFin7MdiLib.Report.WinForm.Program.Show_Mei(); break;
      case "b8": SetDefault(7); new ReviewWindow(_lgr, _bpr, "Mei").Show(); break;
      //case "b9": setDefault(8); new MinFin7MdiLib.DataSet.TxAdd().Show(); break;
      default: _bpr?.No(); break;
    }

    await Task.Delay(500);
    ((Button)s).IsEnabled = true;
  }
  void OnNavigate(object s, System.Windows.Navigation.RequestNavigateEventArgs e)
  {
    var dir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), e.Uri.OriginalString); // ~C:\Users\alexp\OneDrive\Documents\0\MF\DnLds
    try { _ = Process.Start(new ProcessStartInfo(dir)); e.Handled = true; } catch (Exception ex) { _ = ex.Log(dir); }
  }
}