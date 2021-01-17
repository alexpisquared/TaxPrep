using AsLink;
using Db.FinDemo.DbModel;
using System.Data.Entity;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MinFin.Review.DS
{
  public partial class ReviewWindow : AAV.WPF.Base.WindowBase
  {
    A0DbContext _db;
    string _owner; // this is not done yet ... but really any value there?

    public ReviewWindow(string owner) { _owner = owner; InitializeComponent(); }

    void onLoaded(object s, RoutedEventArgs e)
    {
      _db = A0DbContext.Create();
      _db.Vw_TxCore.Load();
      _db.Vw_Exp_Hist_vs_2019.Load();
      dgTxVs.ItemsSource = _db.Vw_Exp_Hist_vs_2019.Local.OrderBy(r => r.Name).ThenBy(r => r.TaxLiq__);
      dgTxVs.Focus();
    }
    void dgCore_SelnChgd(object s, SelectionChangedEventArgs e)
    {
      if (e.AddedItems.Count > 0)
        dbHist.ItemsSource = _db.Vw_TxCore.Local.Where(r => string.Compare(r.TxCategoryIdTxt, ((Vw_Exp_Hist_vs_Last)((object[])e.AddedItems)[0]).IdTxt, true) == 0).OrderByDescending(r => r.TxDate);
    }

    void onUserChecked(object s, RoutedEventArgs e)
    {
      switch (_owner = ((RadioButton)s).Name)
      {
        case "Alx": _db.Vw_Exp_Hist_vs_2018_Alx.Load(); dgTxVs.ItemsSource = _db.Vw_Exp_Hist_vs_2018_Alx.Local.OrderBy(r => r.Name).ThenBy(r => r.TaxLiq__); break;
        case "Mei": _db.Vw_Exp_Hist_vs_2018_Mei.Load(); dgTxVs.ItemsSource = _db.Vw_Exp_Hist_vs_2018_Mei.Local.OrderBy(r => r.Name).ThenBy(r => r.TaxLiq__); break;
        case "Ndn": _db.Vw_Exp_Hist_vs_2018_Ndn.Load(); dgTxVs.ItemsSource = _db.Vw_Exp_Hist_vs_2018_Ndn.Local.OrderBy(r => r.Name).ThenBy(r => r.TaxLiq__); break;
        case "Zoe": _db.Vw_Exp_Hist_vs_2018_Zoe.Load(); dgTxVs.ItemsSource = _db.Vw_Exp_Hist_vs_2018_Zoe.Local.OrderBy(r => r.Name).ThenBy(r => r.TaxLiq__); break;
        default: _db.Vw_Exp_Hist_vs_2019.Load(); dgTxVs.ItemsSource = _db.Vw_Exp_Hist_vs_2019.Local.OrderBy(r => r.Name).ThenBy(r => r.TaxLiq__); break;
      }

      CollectionViewSource.GetDefaultView(dgTxVs.ItemsSource).Refresh(); //tu: refresh bound datagrid
    }
  }
}
