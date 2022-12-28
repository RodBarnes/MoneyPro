using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MoneyPro.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isVisible = (bool)value;

            if (IsVisibilityInverted(parameter))
            {
                isVisible = !isVisible;
            }

            return isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var isVisible = ((Visibility)value == Visibility.Visible);

            if (IsVisibilityInverted(parameter))
            {
                isVisible = !isVisible;
            }

            return isVisible;
        }

        private static Visibility GetVisibilityMode(object parameter)
        {
            // Default to visible
            var mode = Visibility.Visible;

            if (parameter != null)
            {
                if (parameter is Visibility currentState)
                {
                    mode = currentState;
                }
                else
                {
                    try
                    {
                        mode = (Visibility)Enum.Parse(typeof(Visibility), parameter.ToString(), true);
                    }
                    catch (FormatException ex)
                    {
                        throw new FormatException("Invalid Visibility specified as the ConverterParameter.  Use Visible or Collapsed.", ex);
                    }
                }
            }

            return mode;
        }

        private static bool IsVisibilityInverted(object parameter)
        {
            return (GetVisibilityMode(parameter) == Visibility.Collapsed);
        }
    }
}
