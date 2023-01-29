using System;
using System.Windows.Forms;

namespace MinFin.Report.WinForm7//.Mei
{
  public partial class Form1_Mei : Form
  {
    public Form1_Mei() => InitializeComponent();

    void Form1_Load(object s, EventArgs e)
    {
      Vw_TaxLiqReport_MeiTableAdapter.Fill(findemoDataSet.Vw_TaxLiqReport_Mei);

      reportViewer1.RefreshReport();
    }

    void button1_Click(object s, EventArgs e) => Close();
  }
}
