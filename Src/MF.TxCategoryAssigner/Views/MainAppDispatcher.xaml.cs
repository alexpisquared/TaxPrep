using AAV.Sys.Ext;
using AAV.Sys.Helpers;
using MF.TxCategoryAssigner.Properties;
using MSMoneyDbLoader;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MF.TxCategoryAssigner.Views
{
  public partial class MainAppDispatcher : AAV.WPF.Base.WindowBase
  {
    const int _zeroBasedBtnCnt = 8;

    public MainAppDispatcher()
    {
      InitializeComponent();

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

      //Task.Run(async () => await A0DbContext.Create().UnitPrices.LoadAsync()); // preload to ini the EF for faster loads in views.

      setDefault(Settings.Default.LastBtnNo);
      tbver.Text = $"db:{Db.FinDemo.DbModel.A0DbContext.DbName}    -   {VerHelper.CurVerStr(".NET 4.8")}";
    }

    void setDefault(int cb)
    {
      b1.IsDefault = b2.IsDefault = b3.IsDefault = b4.IsDefault = b5.IsDefault = b6.IsDefault = b7.IsDefault = b8.IsDefault = false;
      ((Button)FindName($"b{cb + 1}")).IsDefault = true;
      Settings.Default.LastBtnNo = cb;
      Settings.Default.Save();
      Bpr.ShortFaF();
    }

    async void onX(object s, RoutedEventArgs e)
    {
      ((Button)s).IsEnabled = false;
      switch (((Button)s).Name)
      {
        case "b1": setDefault(0); new DbLoaderReportWindow(MSMoneyDbLoader.App.GetCmndLineArgsInclClickOnce()).ShowDialog(); break;
        case "b2": setDefault(1); new HistoricalChartSet.MainHistChart().Show(); break;
        case "b3": setDefault(2); new TxCategoryAssignerVw().Show(); break;
        case "b4": setDefault(3); new ManualTxnEntry(false).Show(); break;
        case "b5": setDefault(4); MinFin.Report.WinForm.Program.ShowBoth(); break;
        case "b6": setDefault(5); MinFin.Report.WinForm.Program.Show_Alx(); break;
        case "b7": setDefault(6); MinFin.Report.WinForm.Program.Show_Mei(); break;
        case "b8": setDefault(7); new MinFin.Review.DS.ReviewWindow("Mei").Show(); break;
        case "b9": setDefault(8); new MinFin.DataSet.TxAdd().Show(); break;
        default: Bpr.BeepEr(); break;
      }

      await Task.Delay(500);
      ((Button)s).IsEnabled = true;
    }

    void onNavigate(object s, System.Windows.Navigation.RequestNavigateEventArgs e)
    {
      var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), e.Uri.OriginalString); // ~C:\Users\alexp\OneDrive\Documents\0\MF\DnLds
      try { Process.Start(new ProcessStartInfo(dir)); e.Handled = true; } catch (Exception ex) { ex.Log(dir); }
    }

    void BClose_Click(object s, RoutedEventArgs e) => Close();
  }
}
