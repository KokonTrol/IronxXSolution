using Library.Models;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using System.Windows.Media;
using Color = System.Windows.Media.Color;
using ColorConverter = System.Windows.Media.ColorConverter;

namespace StyleResouces
{
    public class StringToDouble : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(double.TryParse(value.ToString(), out double i))
                return double.Parse(value.ToString());
            return "0";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }
    }
    public class IdTypeToTypeName : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IronContext db = new IronContext();
            ProductType? prtype = db.ProductType.Where(p=>p.Id == (int)value).FirstOrDefault();
            if (prtype != null)
                return prtype.Name;
            return "None";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IronContext db = new IronContext();
            ProductType? prtype = db.ProductType.Where(p => p.Name == (string)value).FirstOrDefault();
            if (prtype != null)
                return prtype.Id;
            return "1";
        }
    }
    public class BoolCashToString : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value)
                return "Наличка";
            return "Банковская карта";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString() == "Наличка";
        }
    }

    public class StringToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            System.Windows.Media.Color brush = (Color)ColorConverter.ConvertFromString(value.ToString());
            return brush;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var color = (Color)value;
            string str = string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}",
                        color.A,
                        color.R,
                        color.G,
                        color.B);// "#" + ((Color)value).A.ToString() + ((Color)value).R.ToString() + ((Color)value).G.ToString() + ((Color)value).B.ToString();
            return str;
        }
    }
    public class ColorToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SolidColorBrush brush = new SolidColorBrush((Color)value);
            return brush;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return ((SolidColorBrush)value).Color;
        }
    }
    public class StringToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            SolidColorBrush brush = new SolidColorBrush((Color)ColorConverter.ConvertFromString(value.ToString()));
            return brush;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var color = (Color)value;
            string str = string.Format("#{0:X2}{1:X2}{2:X2}{3:X2}",
                        color.A,
                        color.R,
                        color.G,
                        color.B);
            return str;
        }
    }
}
