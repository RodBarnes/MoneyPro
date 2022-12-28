using System;
using System.Globalization;
using System.Windows.Data;

namespace MoneyPro.Converters
{
    public class BoolToTrueFalseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isTrue = (bool)value;

            return isTrue;
            //return isTrue ? "True" : "False";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var strVal = value.ToString();

            return (strVal == "True");
            //return (strVal == "True");
        }
    }
}
