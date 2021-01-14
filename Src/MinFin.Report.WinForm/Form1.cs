using System;
using System.Windows.Forms;

namespace MinFin.Report.WinForm
{
  public partial class Form1 : Form
  {
    public Form1() => InitializeComponent();

    void Form1_Load(object s, EventArgs e)
    {
      Vw_TaxLiqReport_UnifiedTableAdapter.Fill(findemoDataSet.Vw_TaxLiqReport_Unified);

      reportViewer1.RefreshReport();
    }

    void button1_Click(object s, EventArgs e) => Close();
  }
}
