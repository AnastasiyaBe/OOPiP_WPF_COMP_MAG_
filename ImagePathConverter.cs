using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace ComputerStoreWPF
{
    public class ImagePathConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;
            string path = value.ToString();
            // Убираем префикс /images/, если есть
            if (path.StartsWith("/images/"))
                path = path.Substring(8);
            // Формируем URI к ресурсу, встроенному в сборку
            return new BitmapImage(new Uri($"pack://application:,,,/ComputerStore;component/Images/{path}"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}