using System;
using System.Globalization;
using System.Windows.Data;

namespace MoneyPro.Converters
{
    // This converter handles the null condition to swallow the System 23 and 7 errors
    public class TransactionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if (value != null && value == CollectionView.NewItemPlaceholder)
            //{
            //    //return value;
            //}
            return value;
        }
    }
}
