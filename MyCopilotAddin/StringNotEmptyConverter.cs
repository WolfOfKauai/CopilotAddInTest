// file: E:\Repos\WolfOfKauai\CopilotAddInTest\MyCopilotAddin\StringNotEmptyConverter.cs
using System;
using System.Globalization;
using System.Windows.Data;

namespace MyCopilotAddin
{
  public class StringNotEmptyConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      var str = value as string;
      return !string.IsNullOrWhiteSpace(str);
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotImplementedException();
    }
  }
}