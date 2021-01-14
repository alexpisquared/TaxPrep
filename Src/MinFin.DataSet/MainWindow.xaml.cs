using System.Windows;

namespace MinFin.DataSet
{
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    public MainWindow() => InitializeComponent();

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
/*
 
 BACKUP DATABASE [FinDemo] TO  DISK = N'O:\1\bak\Sql.Bak\FinDemo.Ofc.bak' WITH NOFORMAT, NOINIT, NAME = N'FinDemo-Full Database Backup', SKIP, NOREWIND, NOUNLOAD,  STATS = 10
GO

 * 
 * 
 * 
 * 

SELECT     TOP (200) TxCategory.Id, TxCategory.CreatedAt, TxCategory.Name, SUM(TxCore.TxAmount) AS Expr1
FROM         TxCategory INNER JOIN
                      TxCore ON TxCategory.Id = TxCore.TxCategoryId
WHERE     (TxCore.TxDate > CONVERT(DATETIME, '2010-01-01 00:00:00', 102)) AND (TxCore.TxDate < CONVERT(DATETIME, '2010-12-31 00:00:00', 102))
GROUP BY TxCategory.Id, TxCategory.CreatedAt, TxCategory.Name
ORDER BY TxCategory.Name
 
 
 * 
3	2010-05-15 00:58:40.290	- Yet Unknown -	45.3100
16	2010-05-15 09:26:02.823	Auto Gas	1488.1600
34	2010-06-26 22:20:19.653	Auto Insurance	2452.0000
9	2010-05-15 01:22:44.867	Auto Parking	115.2000
12	2010-05-15 01:33:19.853	Auto Service	149.9600
23	2010-05-18 00:02:13.687	Biz Attire	424.9800
4	2010-05-15 01:08:56.397	Biz Lunch	1094.5700
11	2010-05-15 01:30:19.477	Books	102.4600
13	2010-05-15 01:40:41.583	Chilldren Fitness	463.2500
10	2010-05-15 01:27:02.543	Computer Supplies	7836.3600
33	2010-05-20 20:01:46.013	Donation	198.0000
39	2011-01-22 21:21:04.840	Fitness Club	535.2700
18	2010-05-16 11:49:58.447	Gifts	873.0700
40	2011-01-22 22:44:40.190	Insurance - House	962.2800
36	2010-06-26 22:50:04.313	Insurance - Life	790.0000
37	2010-06-26 22:53:56.520	Misc	21.4800
14	2010-05-15 09:14:32.757	N Music	40.0000
38	2011-01-16 18:25:33.750	Office Furniture	181.7500
15	2010-05-15 09:18:03.670	Office Party	3081.3800
8	2010-05-15 01:20:38.533	Office Supplies	916.4200
24	2010-05-18 00:12:57.430	Postal	116.9700
35	2010-06-26 22:28:34.120	Tax - Property	2981.7500
41	2011-01-25 21:46:59.243	Trip	2758.9800 
 * 
 * 27629.6000 for 2010
 * 32493.7000 for all
 * 
 
 * 
 * 
 * 
 * 
 * 
 * 
SELECT     TOP (200) IdTxt, ExpGroupId, Name, TL_Number, DeductiblePercentage, Notes,
                          (SELECT     SUM(TxAmount) AS Expr1
                            FROM          TxCore
                            WHERE      (TxCategory.Id = TxCategoryId)) AS Expr1
FROM         TxCategory
ORDER BY TL_Number, ExpGroupId, Name
 * 
 * 
 * 
 * 
 * 
 * 
 * * 
 */
