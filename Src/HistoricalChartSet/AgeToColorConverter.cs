using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;

namespace AsLink
{
  public class AgeToColorConverter : MarkupExtension, IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value is DateTime) && !(value is DateTimeOffset)) return new LinearGradientBrush(Colors.Gray, Colors.DarkGray, 45);

      var min = DateTime.Now.AddYears(-2);
      var max = DateTime.Now;

      var val = value is DateTimeOffset ? ((DateTimeOffset)value).DateTime : (DateTime)value;

      if(val < min)
        return new SolidColorBrush(Color.FromRgb(180, 210, 255));

      var g = (byte)((99 + 155 * (val - min).Days / (max - min).Days) % 255);

      Trace.WriteLine($"**> {value}   {val}   {g}");

      return new SolidColorBrush(Color.FromRgb(210, g, 180));
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => Binding.DoNothing;
    public override object ProvideValue(IServiceProvider serviceProvider) => this;
    public AgeToColorConverter() { }
  }
}