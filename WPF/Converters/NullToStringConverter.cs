using System;
using System.Globalization;
using System.Windows.Data;

namespace MoneyPro.Converters
{
    public class NullToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return "";
            }
            else
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                return value;
            }
            else
            {
                return "";
            }
        }
    }
}
