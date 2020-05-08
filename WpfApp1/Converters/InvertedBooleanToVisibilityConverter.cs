using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WpfApp1.Converters
{
    public class InvertedBooleanToVisibilityConverter : IValueConverter
    {
        public static InvertedBooleanToVisibilityConverter Instance = new InvertedBooleanToVisibilityConverter();
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value.GetType() != typeof(bool))
            {
                throw new InvalidCastException("Invalid value type (bool requested)");
            }

            return (bool)value ? Visibility.Collapsed : Visibility.Visible;
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value.GetType() != typeof(Visibility))
            {
                throw new InvalidCastException("Invalid value type (Visibility requested)");
            }

            return (Visibility)value == Visibility.Collapsed;
        }
    }
}