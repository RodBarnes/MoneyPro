using System;
using System.Globalization;
using System.Windows.Data;

namespace MoneyPro.Converters
{
    public class NullToDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return DateTime.Now;
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
                return null;
            }
        }
    }
}
