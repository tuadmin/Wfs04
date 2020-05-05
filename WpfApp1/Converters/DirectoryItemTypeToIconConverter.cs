using System;
using System.Globalization;
using System.Windows.Data;
using MaterialDesignThemes.Wpf;
using WpfApp1.Enums;

namespace WpfApp1.Converters
{
    public class DirectoryItemTypeToIconConverter : IValueConverter
    {
        public static DirectoryItemTypeToIconConverter Instance = new DirectoryItemTypeToIconConverter();
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value.GetType() != typeof(DirectoryItemType))
            {
                throw new InvalidCastException("Invalid value type");
            }

            return value switch
            {
                DirectoryItemType.Drive => PackIconKind.Harddisk,
                DirectoryItemType.Folder => PackIconKind.Folder,
                _ => PackIconKind.File
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}