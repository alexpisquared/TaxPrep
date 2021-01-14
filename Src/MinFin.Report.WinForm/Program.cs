using System;
using System.Windows.Forms;

namespace MinFin.Report.WinForm
{
  public static class Program
  {
    [STAThread]
    public static void Main(string owner)
    {
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);

      switch (owner)
      {
        default: Application.Run(new Form1()); break;
        case "Alx": Application.Run(new Form1_Alx()); break;
        case "Mei": Application.Run(new Form1_Mei()); break;
      }
    }

    public static void ShowBoth() => new Form1().Show();
    public static void Show_Alx() => new Form1_Alx().Show();
    public static void Show_Mei() => new Form1_Mei().Show();
    public static void ShowDialogBoth() => new Form1().ShowDialog();
  }
}
