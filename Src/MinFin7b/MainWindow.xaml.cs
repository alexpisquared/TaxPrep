using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Db.FinDemo7.Models;
using Microsoft.EntityFrameworkCore;

namespace MinFin7b;
public partial class MainWindow : Window
{
  readonly FinDemoContext _db = new();
  public MainWindow()
  {
    InitializeComponent();
  }

  async void OnLoaded(object sender, RoutedEventArgs e)
  {
    try
    {
      Trace.WriteLine($"{_db.TxCategories.Count()}");
      Trace.WriteLine($"{_db.TxCategories.Count()}   {_db.TxCategories.Local.Count()}   ");

      _db.TxCategories.Load();

      Trace.WriteLine($"{_db.TxCategories.Count()}   {_db.TxCategories.Local.Count()}   ");
      dg1.ItemsSource = _db.TxCategories.Local.Take(3);

    }
    catch (Exception ex)
    {
      Trace.WriteLine($"{ex}");
      throw;
    }

  }

  private void OnClose(object sender, RoutedEventArgs e)
  {
    Close();
  }
}
