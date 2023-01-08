using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MinFin7;
using MinFin7.Properties;

namespace MF.TxCategoryAssigner.Views
{
  public partial class MainAppDispatcher : WindowBase
  {
    const int _zeroBasedBtnCnt = 8;
    readonly ILogger<TxCategoryAssignerVw> _logger;
    readonly Bpr _bpr;

    public MainAppDispatcher(ILogger<TxCategoryAssignerVw> logger, Bpr bpr)
    {
      InitializeComponent();

      this._logger = logger;
      this._bpr = bpr;

      KeyUp += (s, e) =>
      {
        switch (e.Key)
        {
          case Key.Escape: Close(); App.Current.Shutdown(); break;
          case Key.Up:   /**/ Settings.Default.LastBtnNo = Settings.Default.LastBtnNo > 0                /**/ ? --Settings.Default.LastBtnNo : 0;                /**/ setDefault(Settings.Default.LastBtnNo); break;
          case Key.Down: /**/ Settings.Default.LastBtnNo = Settings.Default.LastBtnNo < _zeroBasedBtnCnt /**/ ? ++Settings.Default.LastBtnNo : _zeroBasedBtnCnt; /**/ setDefault(Settings.Default.LastBtnNo); break;
          default: break;
        }
      }; //tu:

      //Task.Run(async () => await new().UnitPrices.LoadAsync()); // preload to ini the EF for faster loads in views.

      setDefault(Settings.Default.LastBtnNo);
      //tbver.Text = $"db:{Db.FinDemo7.Models.FinDemoContext.DbName}    -   {VerHelper.CurVerStr(".NET 4.8")}";
    }

    void setDefault(int cb)
    {
      b1.IsDefault = b2.IsDefault = b3.IsDefault = b4.IsDefault = b5.IsDefault = b6.IsDefault = b7.IsDefault = b8.IsDefault = false;
      ((Button)FindName($"b{cb + 1}")).IsDefault = true;
      Settings.Default.LastBtnNo = cb;
      Settings.Default.Save();
      //Bpr.ShortFaF();
    }

    async void onX(object s, RoutedEventArgs e)
    {
      ((Button)s).IsEnabled = false;
      switch (((Button)s).Name)
      {
        //case "b1": setDefault(0); new DbLoaderReportWindow(MSMoneyDbLoader.App.GetCmndLineArgsInclClickOnce()).ShowDialog(); break;
        //case "b2": setDefault(1); new HistoricalChartSet.MainHistChart().Show(); break;
        case "b3": setDefault(2); new TxCategoryAssignerVw(_logger, _bpr).Show(); break;
        case "b4": setDefault(3); new ManualTxnEntry(false).Show(); break;
        //case "b5": setDefault(4); MinFin7.Report.WinForm.Program.ShowBoth(); break;
        //case "b6": setDefault(5); MinFin7.Report.WinForm.Program.Show_Alx(); break;
        //case "b7": setDefault(6); MinFin7.Report.WinForm.Program.Show_Mei(); break;
        //case "b8": setDefault(7); new MinFin7.Review.DS.ReviewWindow("Mei").Show(); break;
        //case "b9": setDefault(8); new MinFin7.DataSet.TxAdd().Show(); break;
        default: break;
      }

      await Task.Delay(500);
      ((Button)s).IsEnabled = true;
    }

    void onNavigate(object s, System.Windows.Navigation.RequestNavigateEventArgs e)
    {
      var dir = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), e.Uri.OriginalString); // ~C:\Users\alexp\OneDrive\Documents\0\MF\DnLds
      try { Process.Start(new ProcessStartInfo(dir)); e.Handled = true; } catch (Exception ex) { ex.Log(dir); }
    }

    void BClose_Click(object s, RoutedEventArgs e) => Close();
  }
}
