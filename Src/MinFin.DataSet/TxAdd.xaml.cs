using Microsoft.Win32;
using MinFin.Report.WinForm;
using System;
using System.Data;
using System.Data.SqlTypes;
using System.Linq;
using System.Speech.Synthesis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MinFin.DataSet
{
  public partial class TxAdd : AAV.WPF.Base.WindowBase
  {
    FinDemoDataSet_Jan2015 minFinDS;
    readonly FinDemoDataSet_Jan2015.TxCoreDataTable _txCoreTblDbValidationCopy = new FinDemoDataSet_Jan2015.TxCoreDataTable();

    public TxAdd() => InitializeComponent();

    void auditAndSave()
    {
      if (!minFinDS.HasChanges())
        return;

      //try { minFinDS.WriteXml(string.Format(@"D:\Users\alex\OneDrive\bak\MinFin\EntryLog.{0}.{1:yyyy.MM.dd.HHmmss}.xml", Environment.MachineName, DateTime.Now)); }
      //catch (Exception ex) { MessageBox.Show(ex.ToString()); }

      var retry = false;
      do
      {
        try
        {
          ///get current total from db 1
          var t1 = new FinDemoDataSet_Jan2015TableAdapters.TxCoreTotalAmountTableAdapter().GetData();
          var dbTtlOld = ((FinDemoDataSet_Jan2015.TxCoreTotalAmountRow)(t1.Rows[0])).TotalTxAmount;

          ///get total of new txs
          var newtxsttl = (decimal)minFinDS.TxCore.Compute("SUM(TxAmount)", ""); ;

          ///save to db
          //int txCategory = new FinDemoDataSet_Jan2015TableAdapters.TxCategoryTableAdapter().Update(minFinDS.TxCategory);
          //int txMoneySrc = new FinDemoDataSet_Jan2015TableAdapters.TxMoneySrcTableAdapter().Update(minFinDS.TxMoneySrc);
          //int txSupplier = new FinDemoDataSet_Jan2015TableAdapters.TxSupplierTableAdapter().Update(minFinDS.TxSupplier);
          var txcore = new FinDemoDataSet_Jan2015TableAdapters.TxCoreTableAdapter().Update(minFinDS.TxCore);

          ///get current total from db 2
          t1 = new FinDemoDataSet_Jan2015TableAdapters.TxCoreTotalAmountTableAdapter().GetData();
          var dbTtlNew = ((FinDemoDataSet_Jan2015.TxCoreTotalAmountRow)(t1.Rows[0])).TotalTxAmount;

          ///assert dbttl 2-2 == ttl of new txs
          if (dbTtlNew - dbTtlOld == newtxsttl)
          {
            ///add audit record
            new FinDemoDataSet_Jan2015TableAdapters.QueriesTableAdapter().CreateAuditRecord("...");
            //MessageBox.Show(string.Format("Audit passed\n\n TxCore rows added: {0} \n Total added ${1:N2} \n\n\n Review before cleanup...", txcore, newtxsttl), "SUCCESS", MessageBoxButton.OK, MessageBoxImage.Information);
            minFinDS.AcceptChanges();
            minFinDS.TxCore.Clear();
            minFinDS.AcceptChanges();
          }
          else
          {
            //report a problem
            retry = MessageBox.Show(string.Format("Audit amounts mismatch ...\r\n\n\nAdded core: {0} rows: \n {1:N2} in db != \n {2:N2} in app", txcore, dbTtlNew - dbTtlOld, newtxsttl), "Crap", MessageBoxButton.OKCancel, MessageBoxImage.Hand) == MessageBoxResult.OK;
          }
        }
        catch (Exception ex) { retry = MessageBox.Show(ex.Message, "Crap", MessageBoxButton.OKCancel, MessageBoxImage.Hand) == MessageBoxResult.OK; }
      } while (retry);
    }
    void emergencyXmlBackupSave()
    {
      try
      {
        var fsds = new FinDemoDataSet_Jan2015();
        //fsds.ReadXml(@"D:\Users\alex\OneDrive\bak\MinFin\EntryLog.VAIO1.2013.01.09.135826.xml");
        foreach (MinFin.DataSet.FinDemoDataSet_Jan2015.TxCoreRow r in fsds.TxCore.Rows)
          minFinDS.TxCore.ImportRow(r);

        auditAndSave();
      }
      catch (Exception ex) { MessageBox.Show(ex.Message, "Unable to add a row"); }
    }

    void Window_Loaded(object s, RoutedEventArgs e)
    {
      minFinDS = ((FinDemoDataSet_Jan2015)(FindResource("minFinDS")));
      //// Load data into the table TxCore. You can modify this code as needed.
      //FinDemoDataSet_Jan2015TableAdapters.TxCoreTableAdapter minFinDSTxCoreTableAdapter = new FinDemoDataSet_Jan2015TableAdapters.TxCoreTableAdapter();
      //minFinDSTxCoreTableAdapter.Fill(minFinDS.TxCore);

      new FinDemoDataSet_Jan2015TableAdapters.TxCategoryTableAdapter().Fill(minFinDS.TxCategory);
      ((CollectionViewSource)(FindResource("txCategoryViewSource"))).View.MoveCurrentToFirst();

      new FinDemoDataSet_Jan2015TableAdapters.TxMoneySrcTableAdapter().Fill(minFinDS.TxMoneySrc);
      ((CollectionViewSource)(FindResource("txMoneySrcViewSource"))).View.MoveCurrentToFirst();

      new FinDemoDataSet_Jan2015TableAdapters.TxSupplierTableAdapter().Fill(minFinDS.TxSupplier);
      ((CollectionViewSource)(FindResource("txSupplierViewSource"))).View.MoveCurrentToFirst();

      new FinDemoDataSet_Jan2015TableAdapters.TxCoreTableAdapter().Fill(_txCoreTblDbValidationCopy);
      new FinDemoDataSet_Jan2015TableAdapters.ExpenseGroupTableAdapter().Fill(minFinDS.ExpenseGroup);

      txAmountTextBox.Focus();

      //var minFinDSTxCoreSimilarTxsTableAdapter = new MinFin.DataSet.FinDemoDataSet_Jan2015TableAdapters.TxCoreSimilarTxsTableAdapter();
      var txCoreSimilarTxsViewSource = ((System.Windows.Data.CollectionViewSource)(FindResource("txCoreSimilarTxsViewSource")));
      txCoreSimilarTxsViewSource.View.MoveCurrentToFirst();

      //worked in Jan2012: emergencyXmlBackupSave();
    }
    void btnAdd_ENTERED_VALUES_AS_TxCoreRecord_Click(object s, RoutedEventArgs e)
    {
      try
      {
        if (tbxNewSup.Text.Trim().Length > 1)
        {
          var r = minFinDS.TxSupplier.AddTxSupplierRow(DateTime.Now, tbxNewSup.Text, "UI added.");
          new FinDemoDataSet_Jan2015TableAdapters.TxSupplierTableAdapter().Update(minFinDS);
          ((CollectionViewSource)(FindResource("txSupplierViewSource"))).View.MoveCurrentToLast();
          txSupplierIdListBox.ScrollIntoView(txSupplierIdListBox.SelectedItem);
          txSupplierIdListBox.IsEnabled = true;
          tbxNewSup.Text = "";
        }

        if (tbxNewCtg.Text.Trim().Length > 1)
        {


          var egr = minFinDS.ExpenseGroup.FirstOrDefault(eg => eg.Id == "Other");
          var r = minFinDS.TxCategory.AddTxCategoryRow(tbxNewCtg.Text,
          egr, //	"Other", 
            DateTime.Now, tbxNewCtg.Text, "UI added.", 999, 1);
          var txCategory = new FinDemoDataSet_Jan2015TableAdapters.TxCategoryTableAdapter().Update(minFinDS);
          ((CollectionViewSource)(FindResource("txCategoryViewSource"))).View.MoveCurrentToLast();
          txCategoryIdListBox.ScrollIntoView(txCategoryIdListBox.SelectedItem);
          txCategoryIdListBox.IsEnabled = true;
          tbxNewCtg.Text = "";
        }

        var txDate = txDateDatePicker.SelectedDate;
        if (txDate == null || txDateDatePicker.Text == "" || txDate.Value < DateTime.Today.AddYears(-10) || txDate.Value > DateTime.Today.AddYears(1)) //bad things happen if date is ouside of SQL accepted range.
        {
          MessageBox.Show("Invalid date", "Oops", MessageBoxButton.OK, MessageBoxImage.Error);
          return;
        }

        if (!decimal.TryParse(txAmountTextBox.Text, out var txAmount))
        {
          MessageBox.Show("Amount is not decimal", "Oops", MessageBoxButton.OK, MessageBoxImage.Error);
          return;
        }

        if ((int)txMoneySrcIdListBox.SelectedValue == 0) { MessageBox.Show("Select MoneySrc", "Oops", MessageBoxButton.OK, MessageBoxImage.Error); return; }
        if ((int)txCategoryIdListBox.SelectedValue == 3) { MessageBox.Show("Select Category", "Oops", MessageBoxButton.OK, MessageBoxImage.Error); return; }
        if ((int)txSupplierIdListBox.SelectedValue == 3) { MessageBox.Show("Select Supplier", "Oops", MessageBoxButton.OK, MessageBoxImage.Error); return; }

        var filter = string.Format("TxAmount={0} and  TxDate='{1}'", txAmount, txDate);
        var dbls = _txCoreTblDbValidationCopy.Select(filter);
        if (dbls.Length > 0)
        {
          MessageBox.Show(
            string.Format("Already spent ${0:N2} on {1} \n\n for {2} {3} \n\n(add time to get it in)",
            ((MinFin.DataSet.FinDemoDataSet_Jan2015.TxCoreRow)(dbls[0])).TxAmount,
            ((MinFin.DataSet.FinDemoDataSet_Jan2015.TxCoreRow)(dbls[0])).TxDate,
            ((MinFin.DataSet.FinDemoDataSet_Jan2015.TxCoreRow)(dbls[0])).ProductService,
            ((MinFin.DataSet.FinDemoDataSet_Jan2015.TxCoreRow)(dbls[0])).Notes),
            "Already there...", MessageBoxButton.OK, MessageBoxImage.Question);
          return;
        }

        try
        {
          var ter = minFinDS.TxEnvelope.FirstOrDefault(te => te.Id == 13);

          var r = minFinDS.TxCore.AddTxCoreRow(
            ((FinDemoDataSet_Jan2015.TxMoneySrcRow)(((DataRowView)(txMoneySrcIdListBox.SelectedItem)).Row)),
            -1,
            ((FinDemoDataSet_Jan2015.TxCategoryRow)(((DataRowView)(txCategoryIdListBox.SelectedItem)).Row)),
            ((FinDemoDataSet_Jan2015.TxSupplierRow)(((DataRowView)(txSupplierIdListBox.SelectedItem)).Row)),
            ter, txAmount, (DateTime)txDate, productServiceTextBox.Text, DateTime.Now, (DateTime)SqlDateTime.MinValue, notesTextBox.Text, "new", txAmount);
          ((CollectionViewSource)(FindResource("txCoreViewSource"))).View.MoveCurrentToLast();

          //txDateDatePicker.Text = "";
          productServiceTextBox.Text = "";
          notesTextBox.Text = "";
          txAmountTextBox.Text = "";

          txAmountTextBox.Focus();

          System.Media.SystemSounds.Asterisk.Play();
        }
        catch (Exception ex) { MessageBox.Show(ex.Message, "Unable to add a row"); }
      }
      catch (Exception ex) { MessageBox.Show(ex.Message, "Unable to add a row"); }
    }
    void btnSaveToDb_Click(object s, RoutedEventArgs e) => auditAndSave();
    void btnReLoadFromDb_Click(object s, RoutedEventArgs e)
    {
    }
    void btnCopyIntoNew_Click(object s, RoutedEventArgs e)
    {
    }
    void btnSaveExit_Click(object s, RoutedEventArgs e)
    {
      if (minFinDS.HasChanges())
      {
        btnSaveToDb_Click(s, null);
      }

      Close();
    }
    void btnLoseChanges_Click(object s, RoutedEventArgs e)
    {
      minFinDS.AcceptChanges(); // lose changes
      Close();
    }
    void Window_Closing(object s, System.ComponentModel.CancelEventArgs e)
    {
      if (minFinDS.HasChanges())
      {
        System.Media.SystemSounds.Question.Play();
        var MessageBoxResult = MessageBox.Show("description of changes", "Save changes?", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
        if (MessageBoxResult == MessageBoxResult.Yes)
          btnSaveToDb_Click(s, null);
        else if (MessageBoxResult == MessageBoxResult.Cancel)
          e.Cancel = true;
        else
          return;
      }
    }
    void tbxNewSup_TextChanged(object s, TextChangedEventArgs e)
    {
      if (((TextBox)s).Text.Length < 1)
        return;

      ((CollectionViewSource)(FindResource("txSupplierViewSource"))).View.MoveCurrentToLast();

      foreach (var v in txSupplierIdListBox.Items)
      {
        if (((MinFin.DataSet.FinDemoDataSet_Jan2015.TxSupplierRow)(((DataRowView)(v)).Row)).Name.ToLower().StartsWith(((TextBox)s).Text.ToLower()))
        {
          txSupplierIdListBox.SelectedItem = v;
          txSupplierIdListBox.ScrollIntoView(v);
          txSupplierIdListBox.IsEnabled = true;
          return;
        }
      }

      txSupplierIdListBox.IsEnabled = false;
    }
    void tbxNewCtg_TextChanged(object s, TextChangedEventArgs e)
    {
      if (((TextBox)s).Text.Length < 1)
        return;

      ((CollectionViewSource)(FindResource("txCategoryViewSource"))).View.MoveCurrentToLast();

      foreach (var v in txCategoryIdListBox.Items)
      {
        if (((MinFin.DataSet.FinDemoDataSet_Jan2015.TxCategoryRow)(((DataRowView)(v)).Row)).Name.ToLower().StartsWith(((TextBox)s).Text.ToLower()))
        {
          txCategoryIdListBox.SelectedItem = v;
          txCategoryIdListBox.ScrollIntoView(v);
          txCategoryIdListBox.IsEnabled = true;
          return;
        }
      }

      txCategoryIdListBox.IsEnabled = false;
    }
    void txAmountTextBox_TextChanged(object s, TextChangedEventArgs e)
    {
      if (txAmountTextBox.Text.Length == 0)
        return;

      if (!decimal.TryParse(txAmountTextBox.Text, out var txAmount))
      {
        MessageBox.Show("Bill Amount is not decimal");
        return;
      }

      if (!decimal.TryParse(tbRnage.Text, out var tsDelta))
      {
        MessageBox.Show("Range Amount is not decimal");
        return;
      }

      new FinDemoDataSet_Jan2015TableAdapters.TxCoreSimilarTxsTableAdapter().FillBy(minFinDS.TxCoreSimilarTxs, tsDelta, txAmount);
      //((CollectionViewSource)(this.FindResource("txCategoryViewSource"))).View.MoveCurrentToFirst();

    }
    void btnLoadBakXml_Click(object s, RoutedEventArgs e)
    {
      var dlg = new OpenFileDialog
      {
        DefaultExt = ".XML",
        InitialDirectory = @"C:\temp",
        Filter = "Text documents (.XML)|*.XML"
      };

      var result = dlg.ShowDialog();
      if (result == true)
      {
        minFinDS.Clear();
        minFinDS.ReadXml(dlg.FileName);
      }
    }
    void txDateDatePicker_DateValidationError(object s, DatePickerDateValidationErrorEventArgs e)
    {
      var foo = new SpeechSynthesizer
      {
        Rate = 2
      };
      foo.SpeakAsync((e.Exception).Message);
    }
    void btnShowReport_Click(object s, RoutedEventArgs e) => new Form1().Show();
  }
}
