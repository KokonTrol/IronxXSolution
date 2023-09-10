using System;
using System.Windows.Data;
using System.Linq;
using Library.Models;

namespace Library.Functions
{
    //конвертация цвета даты в значения в зависимости от текущей даты
    [ValueConversion(typeof(DateTime), typeof(string))]
    public class DateColors : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (((DateTime)value) < DateTime.Today.AddHours(9) && DateTime.Now.Hour >= 9
                || ((DateTime)value) < DateTime.Today.AddDays(-1).AddHours(9) && DateTime.Now.Hour < 9)
                return "#FF0000";
            //else if (((DateTime)value).Date < DateTime.Now.AddHours(-9).Date)
            //    return "#FFAA00";
            else return "#FFFFFF";
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (value.ToString() != "#FFFFFF") return DateTime.Today.AddDays(-1);
            else return DateTime.Today;
        }
    }


    //конвертация даты в значение True/False в зависимости от текущей даты
    [ValueConversion(typeof(DateTime), typeof(bool?))]
    public class DateToggle : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            //IronContext ironContext = new IronContext();
            //if (((DateTime)value).Hour >= 9 && ((DateTime)value).Day == DateTime.Now.Day) //ironContext.GetConfig().DayWork
            //{
            //    return ((DateTime)value).Date == DateTime.Now.AddHours(-9).Date; //ironContext.GetConfig().DayWork
            //}
            //else
            //{
            //    return ((DateTime)value).Date == DateTime.Now.Date;
            //}
            if (((DateTime)value) < DateTime.Today.AddHours(9) && DateTime.Now.Hour >= 9
                || ((DateTime)value) < DateTime.Today.AddDays(-1).AddHours(9) && DateTime.Now.Hour < 9)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if ((bool)value) return DateTime.Today;
            else return DateTime.Today.AddDays(-1);
        }
    }

    //возврат названия ПК относительно ID
    [ValueConversion(typeof(int), typeof(string))]
    public class PCNameFromID : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            var bd = new IronContext();
            return bd.Computers.Where(p => p.Id == System.Convert.ToInt32(value)).First().Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            var bd = new IronContext();
            return bd.Computers.Where(p => p.Name == value.ToString()).First().Id;
        }
    }

    //возврат цвета ПК относительно ID
    [ValueConversion(typeof(int), typeof(string))]
    public class PCColorFromID : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            var bd = new IronContext();
            return bd.Computers.Where(p => p.Id == System.Convert.ToInt32(value)).First().Color;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            var bd = new IronContext();
            return bd.Computers.Where(p => p.Color == value.ToString()).First().Id;
        }
    }

    //возврат названия Игры относительно ID
    [ValueConversion(typeof(int), typeof(string))]
    public class GameNameFromID : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            var bd = new IronContext();
            return bd.Games.Where(p => p.Id == System.Convert.ToInt32(value)).First().Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            var bd = new IronContext();
            return bd.Games.Where(p => p.Name == value.ToString()).First().Id;
        }
    }

    //возврат названия товара относительно ID
    [ValueConversion(typeof(int), typeof(string))]
    public class ProductTypeFromID : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            var bd = new IronContext();
            return bd.ProductType.Where(p => p.Id == System.Convert.ToInt32(value)).First().Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            var bd = new IronContext();
            return bd.ProductType.Where(p => p.Name == value.ToString()).First().Id;
        }
    }

    //возврат имени админа относительно ID
    [ValueConversion(typeof(int), typeof(string))]
    public class AdminNameFromID : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            var bd = new IronContext();
            return bd.Admin.Where(p => p.Id == System.Convert.ToInt32(value)).First().Name;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            var bd = new IronContext();
            return bd.Admin.Where(p => p.Name == value.ToString()).First().Id;
        }
    }

    public class MultiValues : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return values.Clone();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
