using System.Globalization;
using System.Windows.Media;

namespace MinFin7.Views;

public class WeekDayToBkColorConverter : IValueConverter // copy from EPP
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    if (value is not null and string)
    {
      byte upper = 20, lower = 35;
      return ((string)value)[0] switch
      {
        '1' => new LinearGradientBrush(Color.FromArgb(upper, 0x00, 0x00, 0xff), Color.FromArgb(lower, 0x00, 0x00, 0xff), 90),
        '2' => new LinearGradientBrush(Color.FromArgb(upper, 0xFC, 0x00, 0xFF), Color.FromArgb(lower, 0xFC, 0x00, 0xFF), 90),
        '3' => new LinearGradientBrush(Color.FromArgb(upper, 0xaF, 0xf2, 0x00), Color.FromArgb(lower, 0xaF, 0xf2, 0x00), 90),
        '4' => new LinearGradientBrush(Color.FromArgb(upper, 0x00, 0xB8, 0xB8), Color.FromArgb(lower, 0x00, 0xB8, 0xB8), 90),
        '5' => new LinearGradientBrush(Color.FromArgb(upper, 0x01, 0xFF, 0x00), Color.FromArgb(lower, 0x01, 0xFF, 0x00), 90),
        _ => new LinearGradientBrush(Color.FromArgb(upper, 0, 0, 0), Color.FromArgb(lower, 0, 0, 0), 90),
      };
    }

    return new LinearGradientBrush(Colors.Gray, Colors.DarkGray, 45);//		return new BrushConverter().ConvertFromString("#ff0000");
  }
  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
}

public class IncreaseToColorConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    if (value is not null and decimal)
    {
      var c = (decimal)value;
      if (c < 0.8000000m) return new SolidColorBrush(Colors.Blue);
      else if (c < 1.06m) return new SolidColorBrush(Colors.Green);
      else if (c < 9.00m) return new SolidColorBrush(Colors.Red);
      else return new SolidColorBrush(Colors.White);
    }

    return new SolidColorBrush(Colors.Yellow);
  }
  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
}

public class YearColor : IValueConverter // copy from EPP
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    if (value is not null and DateTime)
    {
      byte upper = 20, lower = 35;
      var yearOld = DateTime.Today.Year - ((DateTime)value).Year;
      return (yearOld % 6) switch
      {
        1 => new LinearGradientBrush(Color.FromArgb(upper, 0xCC, 0xCC, 0xCC), Color.FromArgb(lower, 0xCC, 0xCC, 0xCC), 90),
        2 => new LinearGradientBrush(Color.FromArgb(upper, 0xFC, 0x00, 0xFF), Color.FromArgb(lower, 0xFC, 0x00, 0xFF), 90),
        3 => new LinearGradientBrush(Color.FromArgb(upper, 0xaF, 0xf2, 0x00), Color.FromArgb(lower, 0xaF, 0xf2, 0x00), 90),
        4 => new LinearGradientBrush(Color.FromArgb(upper, 0xff, 0x00, 0x00), Color.FromArgb(lower, 0xff, 0x00, 0x00), 90),
        5 => new LinearGradientBrush(Color.FromArgb(upper, 0x01, 0xFF, 0x00), Color.FromArgb(lower, 0x01, 0xFF, 0x00), 90),
        _ => new LinearGradientBrush(Color.FromArgb(upper, 0, 0, 0), Color.FromArgb(lower, 0, 0, 0), 90),
      };
    }

    return new LinearGradientBrush(Colors.Gray, Colors.DarkGray, 45);//		return new BrushConverter().ConvertFromString("#ff0000");
  }
  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
}
