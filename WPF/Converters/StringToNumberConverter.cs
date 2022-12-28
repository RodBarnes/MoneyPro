using System;
using System.Globalization;
using System.Windows.Data;

namespace MoneyPro.Converters
{
    public class StringToNumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return 0;
            }
            else
            {
                return value;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var valid = decimal.TryParse(value.ToString(), out decimal amount);
            if (valid)
            {
                return amount;
            }
            else
            {
                return 0;
            }
        }
    }
}
