using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using WpfApp1.Enums;

namespace WpfApp1.Converters
{
    public class DirectoryItemTypeToImageConverter : IValueConverter
    {
        public static DirectoryItemTypeToImageConverter Instance = new DirectoryItemTypeToImageConverter();
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value.GetType() != typeof(DirectoryItemType))
            {
                throw new InvalidCastException("Invalid value type");
            }

            var image = value switch
            {
                DirectoryItemType.Drive => "hdd.png",
                DirectoryItemType.Folder => "folder.png",
                _ => "file.png"
            };
            
            return new BitmapImage(new Uri($"pack://application:,,,/Resources/Images/{image}"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}