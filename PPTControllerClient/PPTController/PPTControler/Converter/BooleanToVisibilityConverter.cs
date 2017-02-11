using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace PPTController.Converter
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool inverseOperation = false;

            if (parameter != null)
            {
                Boolean.TryParse(parameter.ToString(), out inverseOperation);
            }

            if (inverseOperation)
            {
                return (bool) value ? Visibility.Collapsed : Visibility.Visible;
            }

            return (bool) value ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool inverseOperation = false;

            if (parameter != null)
            {
                Boolean.TryParse(parameter.ToString(), out inverseOperation);
            }

            Visibility visibility = (Visibility) value;

            if (inverseOperation)
            {
                return visibility != Visibility.Visible;
            }

            return visibility == Visibility.Visible;
        }
    }
}
