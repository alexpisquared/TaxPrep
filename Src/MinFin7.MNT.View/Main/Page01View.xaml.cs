﻿namespace MinFin7.MNT.View.Main;
public partial class Page01View : UserControl
{
  public Page01View() { InitializeComponent(); Loaded += async (s, e) => { await Task.Delay(2500); _ = tbFilter.Focus(); }; }
  void OnInitNewItem(object s, InitializingNewItemEventArgs e)
  {
    try
    {
      _ = ((DataGrid)s).Items.MoveCurrentToLast();

      if (((DataGrid)s).SelectedItem != null)
        ((DataGrid)s).ScrollIntoView(((DataGrid)s).SelectedItem);
    }
    catch (Exception ex) { ex.Pop(); }
  }
  void dgPage_SelectionChanged(object s, SelectionChangedEventArgs e)
  {
    try
    {
      if (((DataGrid)s).SelectedItem != null)
        ((DataGrid)s).ScrollIntoView(((DataGrid)s).SelectedItem);
    }
    catch (Exception ex) { ex.Pop(); }
  }
}
