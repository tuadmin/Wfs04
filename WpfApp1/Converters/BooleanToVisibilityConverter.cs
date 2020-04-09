using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WpfApp1.Converters
{
    public class BooleanToVisibilityConverter : IValueConverter
    {
        public static BooleanToVisibilityConverter Instance = new BooleanToVisibilityConverter();
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value.GetType() != typeof(bool))
            {
                throw new InvalidCastException("Invalid value type (bool requested)");
            }

            return (bool)value ? Visibility.Visible : Visibility.Collapsed;
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value.GetType() != typeof(Visibility))
            {
                throw new InvalidCastException("Invalid value type (Visibility requested)");
            }

            return (Visibility)value == Visibility.Visible;
        }
    }
}