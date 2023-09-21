using Library.Functions;
using System;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace IronXHelper
{
    public class FileNameToPath : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            BitmapImage bitmapImage;
            try
            {
                bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri(BaseFunctions.GetDocumentFolder() + "\\Helper\\" + (string)value, UriKind.RelativeOrAbsolute);
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }
            catch
            {
                bitmapImage = new BitmapImage(new Uri("/noimage.jpg", UriKind.Relative)) { CreateOptions = BitmapCreateOptions.IgnoreImageCache };
            }
            return bitmapImage;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}
