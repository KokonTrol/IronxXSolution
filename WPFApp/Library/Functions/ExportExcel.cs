using System;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using Library.Models;
using System.Data;
using System.IO;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.Style;
using System.Windows;
using OfficeOpenXml.Table;

namespace Library.Functions
{
    public static class ExportExcel
    {

        public static async Task<bool> Export(string path, DateTime beginDate, DateTime endDate, bool useWastes = false)
        {
            IronContext db = new IronContext();
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;

            Config config = db.GetConfig();
            beginDate.AddHours(config.DayWork);
            endDate.AddHours(config.DayWork);

            db.Transaction.Load();
            db.Admin.Load();
            db.ProductType.Load();
            db.Waste.Load();

            var package = new ExcelPackage();
            var mainInfo = package.Workbook.Worksheets.Add("Сводка");
            var transactionsShift = package.Workbook.Worksheets.Add("Полная информация");
            var allTransactions = package.Workbook.Worksheets.Add("Все транзакции");
            var allWastes = package.Workbook.Worksheets.Add("Все расходы");




            Task[] tasks = new Task[4]
                {
                    new Task(() => {
                        try
                        {
                            mainInfo = FillMainInfo(beginDate, endDate, config, mainInfo, db, useWastes); 
                        }
                        catch (Exception ex) {
                            MessageBox.Show($"Ошибка сохранения на страницу \"Сводка\".\n{ex.Message}");
                        }
                    }),
                    new Task(() => {
                        try
                        {
                            transactionsShift = FillTransactionsShift(beginDate, endDate, config, transactionsShift, db);}
                        catch (Exception ex) {
                            MessageBox.Show($"Ошибка сохранения на страницу \"Полная информация\".\n{ex.Message}");
                        }
                    }),
                    new Task(() => {
                        try
                        {
                            allTransactions = FillAllTransactions(beginDate, endDate, config, allTransactions, db);
                        }
                        catch (Exception ex) {
                            MessageBox.Show($"Ошибка сохранения на страницу \"Все транзакции\".\n{ex.Message}");
                        }
                    }),
                    new Task(() => {
                        try
                        {
                            if (useWastes)
                            { allWastes = FillAllWastes(beginDate, endDate, config, allWastes); }
                            else{package.Workbook.Worksheets.Delete(allWastes); }
                        }
                        catch (Exception ex) {
                            MessageBox.Show($"Ошибка сохранения на страницу \"Все расходы\".\n{ex.Message}");
                        }
                    })
                };
            foreach (var t in tasks)
                t.Start();
            Task.WaitAll(tasks);
            try
            {
                package.SaveAs(new FileInfo(@$"{path}"));
            }
            catch
            {
                MessageBox.Show("Произошла ошибка при сохранении. Проверьте, закрыт ли файл для перезаписи и правильно ли указан путь сохранения.",
                    "Предупреждение",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private static ExcelWorksheet FillTransactionsShift(DateTime beginDate, 
                                                    DateTime endDate, 
                                                    Config config,
                                                    ExcelWorksheet fullInfo, 
                                                    IronContext db)
        {
            int row = 1;
            int column = 1;
            DateTime tempDate = beginDate;

            //проход по каждому дню
            while (tempDate.Date != endDate.AddDays(1).Date)
            {
                var trTemp = db.Transaction.Local.Where(t =>
                                            t.Date <= tempDate.AddDays(1) &&
                                            t.Date >= tempDate)
                                            .GroupBy(t => t.ProductTypeId);
                //проверка на наличие транзакций
                if (trTemp.Count() == 0)
                {
                    tempDate = tempDate.AddDays(1);
                    continue;
                }

                //написание даты
                fullInfo.Cells[row, column, row, column + 3].Merge = true;
                fullInfo.Cells[row, column].Value = tempDate.Date.Date.ToString("dd.MM.yyyy");
                row++;

                //печать данных о каждом товаре
                foreach (var item in trTemp)
                {
                    //инфо о товаре
                    fullInfo.Cells[row, column, row, column + 3].Merge = true;
                    fullInfo.Cells[row, column].Value = db.ProductType.Local.Where(p => p.Id == item.Key).First().Name;
                    fullInfo.Cells[row, column].Style.Font.Bold = true;
                    row++;

                    //заголовки мини-таблиц
                    fullInfo.Cells[row, column + 1, row, column + 3].LoadFromArrays(new object[][] { new[]
                    {
                        "Наличка", "Безнал", "Админ"
                    } });
                    fullInfo.Cells[row, column + 1, row, column + 3].Style.Font.Bold = true;
                    row++;

                    fullInfo.Cells[row, column].Value = "День";
                    fullInfo.Cells[row + 1, column].Value = "Ночь";
                    fullInfo.Cells[row, column, row + 1, column].Style.Font.Bold = true;

                    //запись суммы дневных транзакций
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

                    //запись суммы ночных транзакций
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
                fullInfo.Cells[1, 1, fullInfo.Dimension.End.Row, fullInfo.Dimension.End.Column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                fullInfo.Cells[1, 1, fullInfo.Dimension.End.Row, fullInfo.Dimension.End.Column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                fullInfo.Cells[1, 1, fullInfo.Dimension.End.Row, fullInfo.Dimension.End.Column].Style.WrapText = true;

                tempDate = tempDate.AddDays(1);
            }
            return fullInfo;
        }
        private static ExcelWorksheet FillMainInfo(DateTime beginDate,
                                                    DateTime endDate,
                                                    Config config,
                                                    ExcelWorksheet sheet,
                                                    IronContext db,
                                                    bool useWastes)
        {

            var TransactionOrderByProducts = db.Transaction.Local
                                            .Where(t => t.Date >= beginDate && t.Date < endDate.AddDays(1))
                                            .GroupBy(t => t.ProductTypeId);

            int row = 1;
            int column = 1;

            //заголовок
            sheet.Cells[row, column, row, column + 1].Merge = true;
            sheet.Cells[row, column].Value = $"Даты данных для отчета: {beginDate.ToString("dd.MM.yyyy")} - {endDate.ToString("dd.MM.yyyy")}";
            sheet.Cells[row, column].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
            row++;

            double totalTransSummCash = db.Transaction.Local
                                            .Where(t => t.Date >= beginDate && t.Date < endDate.AddDays(1) && t.IsCash == true).Sum(i => i.Paid);
            double totalTransSummNonCash = db.Transaction.Local
                                            .Where(t => t.Date >= beginDate && t.Date < endDate.AddDays(1) && t.IsCash == false).Sum(i => i.Paid);

            //табличка доходов
            #region transactions
            //начало доходов
            sheet.Cells[row, column, row, column + 1].Merge = true;
            sheet.Cells[row, column].Value = "Доходы";
            sheet.Cells[row, column].Style.Font.Bold = true;
            sheet.Cells[row, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            row++;

            sheet.Cells[row, column, row, column + 1].LoadFromArrays(
                        new object[][] { new[] { "Наличка", "Безнал" } });
            sheet.Cells[row, column + 1, row, column + 1].Style.Font.Bold = true;
            sheet.Cells[row, column + 1, row, column + 1].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
            row++;
            //запись сумм
            sheet.Cells[row, column].Value = totalTransSummCash;
            sheet.Cells[row, column + 1].Value = totalTransSummNonCash;
            //сумма
            row++;
            sheet.Cells[row, column, row, column + 1].Merge = true;
            sheet.Cells[row, column].Value = totalTransSummNonCash + totalTransSummCash;


            sheet.Cells[row - 2, column, row, column + 1].Style.Border.Top.Style = ExcelBorderStyle.Thin;
            sheet.Cells[row - 2, column, row, column + 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
            sheet.Cells[row - 2, column, row, column + 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;
            sheet.Cells[row - 2, column, row, column + 1].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
            row += 2;
            #endregion

            //информация о тратах
            #region wastes
            if (useWastes)
            {
                //начало трат
                sheet.Cells[row, column, row, column + 2].Merge = true;
                sheet.Cells[row, column].Value = "Расходы/Прибыль";
                sheet.Cells[row, column].Style.Font.Bold = true;
                row++;

                sheet.Cells[row, column].Value = "Расходы";
                sheet.Cells[row+1, column].Value = "Прибыль";
                sheet.Cells[row, column, row + 1, column].Style.Font.Bold = true;
                sheet.Cells[row, column, row + 1, column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                //запись сумм
                double totalWastes = db.Waste.Local
                                            .Where(t => t.Date >= beginDate && t.Date < endDate.AddDays(1)).Sum(i => i.Value);
                sheet.Cells[row, column+1].Value = totalWastes;
                sheet.Cells[row+1, column + 1].Value = totalTransSummCash + totalTransSummNonCash - totalWastes;
                row++;
                sheet.Cells[row - 1, column, row, column + 1].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                sheet.Cells[row - 1, column, row, column + 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                sheet.Cells[row - 1, column, row, column + 1].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                sheet.Cells[row - 1, column, row, column + 1].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                row += 2;
            }
            #endregion

            //заголовок
            sheet.Cells[row, column, row, column + 3].Merge = true;
            sheet.Cells[row, column].Value = $"Информация по каждой группе товаров:";
            sheet.Cells[row, column].Style.Font.Bold = true;

            row++;

            //создание таблицы о доходах по товарам
            DataTable dataTable = new DataTable();
            dataTable.Columns.Add("Товар", typeof(string));
            dataTable.Columns.Add("Наличка", typeof(string));
            dataTable.Columns.Add("Безнал", typeof(string));
            dataTable.Columns.Add("Итого", typeof(string));

            foreach (var item in TransactionOrderByProducts)
            {
                dataTable.Rows.Add(db.ProductType.Local.Where(p => p.Id == item.Key).First().Name,
                    item.Where(i => i.IsCash == true).Sum(i => i.Paid),
                    item.Where(i => i.IsCash == false).Sum(i => i.Paid),
                    item.Sum(i => i.Paid));
            }
            sheet.Cells[row, 1].LoadFromDataTable(dataTable, true);
            ExcelRange range = sheet.Cells[row, 1, row + dataTable.Rows.Count, dataTable.Columns.Count];
            ExcelTable tab = sheet.Tables.Add(range, "MainInfo");
            tab.TableStyle = TableStyles.Medium3;

            sheet.Cells[1, 1, sheet.Dimension.End.Row, sheet.Dimension.End.Column].AutoFitColumns();
            sheet.Cells[1, 1, sheet.Dimension.End.Row, sheet.Dimension.End.Column].Style.WrapText = true;
            sheet.Cells[1, 1, sheet.Dimension.End.Row, sheet.Dimension.End.Column].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

            return sheet;
        }
        private static ExcelWorksheet FillAllTransactions(DateTime beginDate,
                                                            DateTime endDate,
                                                            Config config,
                                                            ExcelWorksheet tableInfo,
                                                            IronContext db)
        {
            var AllTransactions = db.Transaction.Local
                                            .Where(t => t.Date >= beginDate && t.Date < endDate.AddDays(1));

            //создание таблицы
            DataTable dataTable = new DataTable();
            //добавление заголовков
            dataTable.Columns.Add("ДАТА", typeof(string));
            dataTable.Columns.Add("ВРЕМЯ", typeof(string));
            dataTable.Columns.Add("ПРОДАЖА", typeof(string));
            dataTable.Columns.Add("СУММА", typeof(double));
            dataTable.Columns.Add("ТИП ПЛАТЫ", typeof(string));
            dataTable.Columns.Add("АДМИН", typeof(string));

            //заполнение
            foreach (var item in AllTransactions)
            {
                dataTable.Rows.Add(item.Date.Date.ToString("dd.MM.yyyy"),
                       item.Date.ToString("HH:mm:ss"),
                       db.ProductType.Where(p => p.Id == item.ProductTypeId).First().Name,
                       item.Paid as object,
                       item.IsCash ? "Наличка" : "Безнал",
                       db.Admin.Where(a => a.Id == item.AdminId).First().Name);
            }
            tableInfo.Cells["A1"].LoadFromDataTable(dataTable, true);
            ExcelRange range = tableInfo.Cells[1, 1, tableInfo.Dimension.End.Row, tableInfo.Dimension.End.Column];
            ExcelTable tab = tableInfo.Tables.Add(range, "AllTransactions");
            tab.TableStyle = TableStyles.Medium3;

            tableInfo.Cells[1, 1, tableInfo.Dimension.End.Row, tableInfo.Dimension.End.Column].AutoFitColumns();
            return tableInfo;
        }

        private static ExcelWorksheet FillAllWastes(DateTime beginDate,
                                                            DateTime endDate,
                                                            Config config,
                                                            ExcelWorksheet wastesTable)
        {
            IronContext db = new IronContext();
            db.Waste.Load();
            var AllWastes = db.Waste.Local.Where(t => t.Date >= beginDate && t.Date < endDate.AddDays(1));

            //создание таблицы
            DataTable dataTable = new DataTable();
            //добавление заголовков
            dataTable.Columns.Add("ДАТА ДОБАВЛЕНИЯ", typeof(string));
            dataTable.Columns.Add("ВРЕМЯ", typeof(string));
            dataTable.Columns.Add("ПРИЧИНА", typeof(string));
            dataTable.Columns.Add("СУММА", typeof(double));

            //заполнение
            foreach (var item in AllWastes)
            {
                dataTable.Rows.Add(item.Date.Date.ToString("dd.MM.yyyy"),
                       item.Date.ToString("HH:mm:ss"),
                       item.Reason,
                       item.Value );
            }
            wastesTable.Cells["A1"].LoadFromDataTable(dataTable, true);
            ExcelRange range = wastesTable.Cells[1, 1, wastesTable.Dimension.End.Row, wastesTable.Dimension.End.Column];
            ExcelTable tab = wastesTable.Tables.Add(range, "AllTransactions");
            tab.TableStyle = TableStyles.Medium3;

            wastesTable.Cells[1, 1, wastesTable.Dimension.End.Row, wastesTable.Dimension.End.Column].AutoFitColumns();
            return wastesTable;
        }
    }
}
