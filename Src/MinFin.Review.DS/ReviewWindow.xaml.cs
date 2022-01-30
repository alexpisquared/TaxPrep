using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using AAV.Sys.Helpers;
using Db.FinDemo.DbModel;

namespace MinFin.Review.DS
{
  public partial class ReviewWindow : AAV.WPF.Base.WindowBase
  {
    A0DbContext _db;
    string _owner;

    public ReviewWindow(string owner) { _owner = owner; InitializeComponent(); }

    async void onLoaded(object s, RoutedEventArgs e)
    {
      Bpr.Beep1of2();
      _db = A0DbContext.Create();
      await Task.Yield(); // it really shows window on .Net 4.8 !!! (2022-Jan-30)
      await _db.Vw_TxCore.LoadAsync()/*.ConfigureAwait(false)*/;            // TxCoreV2 would show the MoneySrc.Name in Binding
      await _db.Vw_Exp_Hist_vs_Last.LoadAsync()/*.ConfigureAwait(false)*/;
      dgTxVs.ItemsSource = _db.Vw_Exp_Hist_vs_Last.Local.OrderBy(r => r.Name).ThenBy(r => r.TaxLiq__);
      dbHist.ItemsSource = null;
      dgTxVs.Focus();
      Bpr.Beep2of2();
    }
    void dgCore_SelnChgd(object s, SelectionChangedEventArgs e)
    {
      if (e.AddedItems.Count > 0)
      {
        Bpr.BeepOk();
        dbHist.ItemsSource = _db.Vw_TxCore.Local.Where(r => string.Compare(r.TxCategoryIdTxt, ((Vw_Exp_Hist_vs_Last)((object[])e.AddedItems)[0]).IdTxt, true) == 0).OrderByDescending(r => r.TxDate);
      }
      else
        Bpr.BeepNo();
    }

    void onUserChecked(object s, RoutedEventArgs e)
    {
      switch (_owner = ((RadioButton)s).Name)
      {
        case "Alx": _db.Vw_Exp_Hist_vs_2018_Alx.Load(); dgTxVs.ItemsSource = _db.Vw_Exp_Hist_vs_2018_Alx.Local.OrderBy(r => r.Name).ThenBy(r => r.TaxLiq__); break;
        case "Mei": _db.Vw_Exp_Hist_vs_2018_Mei.Load(); dgTxVs.ItemsSource = _db.Vw_Exp_Hist_vs_2018_Mei.Local.OrderBy(r => r.Name).ThenBy(r => r.TaxLiq__); break;
        case "Ndn": _db.Vw_Exp_Hist_vs_2018_Ndn.Load(); dgTxVs.ItemsSource = _db.Vw_Exp_Hist_vs_2018_Ndn.Local.OrderBy(r => r.Name).ThenBy(r => r.TaxLiq__); break;
        case "Zoe": _db.Vw_Exp_Hist_vs_2018_Zoe.Load(); dgTxVs.ItemsSource = _db.Vw_Exp_Hist_vs_2018_Zoe.Local.OrderBy(r => r.Name).ThenBy(r => r.TaxLiq__); break;
        default: _db.Vw_Exp_Hist_vs_Last.Load(); dgTxVs.ItemsSource = _db.Vw_Exp_Hist_vs_Last.Local.OrderBy(r => r.Name).ThenBy(r => r.TaxLiq__); break;
      }

      CollectionViewSource.GetDefaultView(dgTxVs.ItemsSource).Refresh(); //tu: refresh bound datagrid
    }
  }
}
