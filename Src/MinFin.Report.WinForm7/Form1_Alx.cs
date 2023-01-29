using System;
using System.Windows.Forms;

namespace MinFin.Report.WinForm7//.Alx
{
  public partial class Form1_Alx : Form
  {
    public Form1_Alx() => InitializeComponent();

    void Form1_Load(object s, EventArgs e)
    {
      Vw_TaxLiqReport_AlxTableAdapter.Fill(findemoDataSet.Vw_TaxLiqReport_Alx);

      reportViewer1.RefreshReport();
    }

    void button1_Click(object s, EventArgs e) => Close();
  }
}
