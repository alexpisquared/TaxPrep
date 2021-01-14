using System.Windows;

namespace MinFin.DataSet
{
  /// <summary>
  /// Interaction logic for TxEdit.xaml
  /// </summary>
  public partial class TxEdit : Window
  {
    public TxEdit() => InitializeComponent();

    void Window_Loaded(object s, RoutedEventArgs e)
    {

      var minFinDS = ((MinFin.DataSet.FinDemoDataSet_Jan2015)(FindResource("minFinDS")));
      // Load data into the table TxCore. You can modify this code as needed.
      var minFinDSTxCoreTableAdapter = new MinFin.DataSet.FinDemoDataSet_Jan2015TableAdapters.TxCoreTableAdapter();
      minFinDSTxCoreTableAdapter.Fill(minFinDS.TxCore);
      var txCoreViewSource = ((System.Windows.Data.CollectionViewSource)(FindResource("txCoreViewSource")));
      txCoreViewSource.View.MoveCurrentToFirst();
      // Load data into the table TxCategory. You can modify this code as needed.
      var minFinDSTxCategoryTableAdapter = new MinFin.DataSet.FinDemoDataSet_Jan2015TableAdapters.TxCategoryTableAdapter();
      minFinDSTxCategoryTableAdapter.Fill(minFinDS.TxCategory);
      var txCategoryViewSource = ((System.Windows.Data.CollectionViewSource)(FindResource("txCategoryViewSource")));
      txCategoryViewSource.View.MoveCurrentToFirst();
      // Load data into the table TxMoneySrc. You can modify this code as needed.
      var minFinDSTxMoneySrcTableAdapter = new MinFin.DataSet.FinDemoDataSet_Jan2015TableAdapters.TxMoneySrcTableAdapter();
      minFinDSTxMoneySrcTableAdapter.Fill(minFinDS.TxMoneySrc);
      var txMoneySrcViewSource = ((System.Windows.Data.CollectionViewSource)(FindResource("txMoneySrcViewSource")));
      txMoneySrcViewSource.View.MoveCurrentToFirst();
      // Load data into the table TxSupplier. You can modify this code as needed.
      var minFinDSTxSupplierTableAdapter = new MinFin.DataSet.FinDemoDataSet_Jan2015TableAdapters.TxSupplierTableAdapter();
      minFinDSTxSupplierTableAdapter.Fill(minFinDS.TxSupplier);
      var txSupplierViewSource = ((System.Windows.Data.CollectionViewSource)(FindResource("txSupplierViewSource")));
      txSupplierViewSource.View.MoveCurrentToFirst();
    }

    void btnAddSup_Click(object s, RoutedEventArgs e)
    {

    }

    void btnAddCtg_Click(object s, RoutedEventArgs e)
    {

    }

    void btnAddBlankTxCoreRecord_Click(object s, RoutedEventArgs e)
    {

    }

    void btnSaveToDb_Click(object s, RoutedEventArgs e)
    {

    }

    void btnReLoadFromDb_Click(object s, RoutedEventArgs e)
    {

    }

    void btnCopyIntoNew_Click(object s, RoutedEventArgs e)
    {

    }
  }
}
