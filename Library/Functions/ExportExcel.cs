using System;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using Library.Models;
using System.Data;
using System.IO;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using System.Globalization;
using Xceed.Wpf.Toolkit.PropertyGrid.Attributes;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using OfficeOpenXml.FormulaParsing.Excel.Functions.RefAndLookup;
using System.Threading;
using System.Windows;

namespace Library.Functions
{
    public static class ExportExcel
    {

        public static async Task<bool> Export(string path, DateTime beginDate, DateTime endDate)
        {
            IronContext db = new IronContext();

            Config config = db.GetConfig();
            beginDate.AddHours(config.DayWork);
            endDate.AddHours(config.DayWork);

            db.Transaction.Load();
            db.Admin.Load();
            db.ProductType.Load();

            var package = new ExcelPackage();
            var sheet = package.Workbook.Worksheets.Add("Сводка");
            var fullInfo = package.Workbook.Worksheets.Add("Полная информация");
            var tableInfo = package.Workbook.Worksheets.Add("Все транзакции");



            Task[] tasks = new Task[3]
                {
                    new Task(() => FillMainInfo(beginDate,
                                                    endDate,
                                                    config,
                                                    ref sheet,
                                                    db)),
                    new Task(() => FillTransactionsShift(beginDate,
                                                    endDate,
                                                    config,
                                                    ref fullInfo,
                                                    db)),
                    new Task(() => FillAllTransactions(beginDate,
                                                    endDate,
                                                    config,
                                                    ref tableInfo,
                                                    db))
                };
            foreach (var t in tasks)
                t.Start();
            Task.WaitAll(tasks);
            try
            {
                File.WriteAllBytes(path, package.GetAsByteArray());
            }
            catch
            {
                MessageBox.Show("Произошла ошибка при сохранении. Проверьте, закрыт ли файл для перезаписи и правильно ли указан путь сохранения.",
                    "Предупреждение",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                    return false;
            }
            //MessageBox.Show("Экспорт данных завершен");
            return true;
        }

        private static void FillTransactionsShift(DateTime beginDate, 
                                                    DateTime endDate, 
                                                    Config config,
                                                    ref ExcelWorksheet fullInfo, 
                                                    IronContext db)
        {
            //var fullInfo = package.Workbook.Worksheets.Add("Полная информация");
            fullInfo.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            fullInfo.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            

            int row = 1;
            int column = 1;
            DateTime tempDate = beginDate;

            while (tempDate.Date != endDate.AddDays(1).Date)
            {
                var trTemp = db.Transaction.Local.Where(t =>
                                            t.Date <= tempDate.AddDays(1) &&
                                            t.Date >= tempDate)
                                            .GroupBy(t => t.ProductTypeId);
                if (trTemp.Count() == 0)
                {
                    continue;
                }
                fullInfo.Cells[row, column].Value = tempDate.Date;
                fullInfo.Cells[row, column].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.LongDatePattern;
                fullInfo.Cells[row, column, row, column + 3].Merge = true;

                fullInfo.Cells.Style.WrapText = true;
                row++;

                

                foreach (var item in trTemp)
                {
                    fullInfo.Cells[row, column, row, column + 3].Merge = true;
                    fullInfo.Cells[row, column].Value = db.ProductType.Local.Where(p => p.Id == item.Key).First().Name;
                    fullInfo.Cells[row, column].Style.Font.Bold = true;
                    fullInfo.Cells[row, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    row++;

                    fullInfo.Cells[row, column + 1, row, column + 3].LoadFromArrays(new object[][] { new[]
                    {
                        "Наличка", "Безнал", "Админ"
                    } });
                    fullInfo.Cells[row, column + 1, row, column + 3].Style.Font.Bold = true;
                    fullInfo.Cells[row, column + 1, row, column + 3].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    row++;

                    fullInfo.Cells[row, column].Value = "День";
                    fullInfo.Cells[row + 1, column].Value = "Ночь";
                    fullInfo.Cells[row, column, row + 1, column].Style.Font.Bold = true;
                    fullInfo.Cells[row, column, row + 1, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                    var dayTrans = item.Where(i =>
                    i.Date < tempDate.AddHours(config.NightWork - config.DayWork)).ToList();

                    fullInfo.Cells[row, column + 1, row, column + 3].LoadFromArrays(new object[][] { new[]
                    {
                       dayTrans.Where(i =>
                                    i.IsCash == true)
                                .Sum(i => i.Paid) as object,
                       dayTrans.Where(i =>
                                    i.IsCash == false)
                                .Sum(i => i.Paid) as object,
                       string.Join(", ", db.Admin.Local.Where(a =>
                                   dayTrans.Select(x => x.AdminId).ToList().Contains(a.Id))
                                   .Select(x=>x.Name).ToArray())
                            } 
                    });

                    row++;

                    var nightTrans = item.Where(i =>
                                                i.Date >= tempDate.AddHours(config.NightWork - config.DayWork)).ToList();
                    fullInfo.Cells[row, column + 1, row, column + 3].LoadFromArrays(new object[][] { new[]
                    {
                       nightTrans.Where(i =>
                                    i.IsCash == true)
                                .Sum(i => i.Paid) as object,
                       nightTrans.Where(i =>
                                    i.IsCash == false)
                                .Sum(i => i.Paid) as object,
                       string.Join(", ", db.Admin.Local.Where(a =>
                                   nightTrans.Select(x => x.AdminId).ToList().Contains(a.Id))
                        .Select(x=>x.Name).ToArray())
                    } });
                    fullInfo.Cells[row - 2, column, row, column + 3].Style.Border.BorderAround(ExcelBorderStyle.Thick);
                    row++;
                }

                row++;

                tempDate = tempDate.AddDays(1);
            }
        }
        private static void FillMainInfo(DateTime beginDate,
                                                    DateTime endDate,
                                                    Config config,
                                                    ref ExcelWorksheet sheet,
                                                    IronContext db)
        {

            var TransactionOrderByProducts = db.Transaction.Local
                                            .Where(t => t.Date >= beginDate && t.Date < endDate.AddDays(1))
                                            .GroupBy(t => t.ProductTypeId);
            //var sheet = package.Workbook.Worksheets.Add("Сводка");
            sheet.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            sheet.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            int row = 1;
            int column = 1;

            sheet.Cells[row, column, row + 1, column + 2].Merge = true;
            sheet.Cells[row, column].Value = $"Даты данных для отчета: {beginDate.ToString("dd.MM.yyyy")} - {endDate.ToString("dd.MM.yyyy")}";
            sheet.Cells.Style.WrapText = true;
            row++;
            row++;


            sheet.Cells[row, column, row, column + 2].Merge = true;
            sheet.Cells[row, column].Value = "Всего";
            sheet.Cells[row, column].Style.Font.Bold = true;
            sheet.Cells[row, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            row++;

            sheet.Cells[row, column + 1, row, column + 2].LoadFromArrays(new object[][] { new[]
                    {
                        "Наличка", "Безнал"
                    } });
            sheet.Cells[row, column + 1, row, column + 2].Style.Font.Bold = true;
            sheet.Cells[row, column + 1, row, column + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            row++;

            sheet.Cells[row, column + 1].Value = db.Transaction.Local
                                            .Where(t => t.Date >= beginDate && t.Date < endDate.AddDays(1) && t.IsCash == true).Sum(i => i.Paid);
            sheet.Cells[row, column + 2].Value = db.Transaction.Local
                                            .Where(t => t.Date >= beginDate && t.Date < endDate.AddDays(1) && t.IsCash == false).Sum(i => i.Paid);

            sheet.Cells[row - 1, column, row, column + 2].Style.Border.BorderAround(ExcelBorderStyle.Thick);

            row += 2;

            sheet.Cells[row, column].Value = $"Информация по каждой группе товаров:";
            sheet.Cells[row, column].Style.Font.Size = 20;
            sheet.Cells[row, column].Style.WrapText = false;
            sheet.Cells[row, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Left;


            row += 2;

            foreach (var item in TransactionOrderByProducts)
            {
                sheet.Cells[row, column, row, column + 2].Merge = true;
                sheet.Cells[row, column].Value = db.ProductType.Local.Where(p => p.Id == item.Key).First().Name;
                sheet.Cells[row, column].Style.Font.Bold = true;
                sheet.Cells[row, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                row++;

                sheet.Cells[row, column + 1, row, column + 2].LoadFromArrays(new object[][] { new[]
                    {
                        "Наличка", "Безнал"
                    } });
                sheet.Cells[row, column + 1, row, column + 2].Style.Font.Bold = true;
                sheet.Cells[row, column + 1, row, column + 2].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                row++;

                sheet.Cells[row, column + 1].Value = item.Where(i => i.IsCash == true).Sum(i => i.Paid);
                sheet.Cells[row, column + 2].Value = item.Where(i => i.IsCash == false).Sum(i => i.Paid);

                sheet.Cells[row - 1, column, row, column + 2].Style.Border.BorderAround(ExcelBorderStyle.Thick);
                row += 2;
            }
        }

        private static void FillAllTransactions(DateTime beginDate,
                                                    DateTime endDate,
                                                    Config config,
                                                    ref ExcelWorksheet tableInfo,
                                                    IronContext db)
        {

            var AllTransactions = db.Transaction.Local
                                            .Where(t => t.Date >= beginDate && t.Date < endDate.AddDays(1));
            //var sheet = package.Workbook.Worksheets.Add("Сводка");
            tableInfo.Cells.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            tableInfo.Cells.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            int row = 1;
            int column = 1;

            //Заголовки
            tableInfo.Cells[row, column, row, column + 4].LoadFromArrays(new object[][] { new[]
                    {
                       "ДАТА",
                       "ПРОДАЖА",
                       "СУММА",
                       "ТИП ПЛАТЫ",
                       "АДМИН"
                    }
            });
            tableInfo.Cells[row, column, row, column + 4].Style.Font.Bold = true;

            row ++;

            foreach (var item in AllTransactions)
            {

                tableInfo.Cells[row, column, row, column + 4].LoadFromArrays(new object[][] { new[] {
                       item.Date,
                       db.ProductType.Where(p => p.Id == item.ProductTypeId).First().Name,
                       item.Paid as object,
                       item.IsCash ? "Наличка" : "Безнал",
                       db.Admin.Where(a => a.Id == item.AdminId).First().Name
                    } }
                );
                tableInfo.Cells[row, column].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.LongDatePattern;

                row++;
            }
        }
    }
}
