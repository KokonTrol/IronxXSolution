﻿using Microsoft.Win32;
using System.Windows;
using Library.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Library.Functions;
using System;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using System.IO;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Database;
using System.Diagnostics;

namespace IronxXSolution
{
    public partial class SettingExcel : Window
    {
        public SettingExcel(DateTime? minDate = null, DateTime? maxDate = null)
        {
            IronContext ironContext = new IronContext();
            ironContext.Transaction.Load();
            InitializeComponent();

            if (minDate != null && maxDate != null)
            {
                BeginDate.SelectedDate = minDate;
                EndDate.SelectedDate = maxDate;
            }
            else
            {
                //var min = ironContext.Transaction.Min(t => t.Date);
                //var max = ironContext.Transaction.Max(t => t.Date);

                BeginDate.DisplayDateEnd = EndDate.DisplayDateEnd = ironContext.Transaction.Max(t => t.Date).Date;
                BeginDate.DisplayDateStart = EndDate.DisplayDateStart = ironContext.Transaction.Min(t => t.Date).Date;
                BeginDate.SelectedDate = EndDate.SelectedDate = EndDate.DisplayDateEnd;
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (BeginDate.SelectedDate.Value <= EndDate.SelectedDate.Value)
            {
                SaveFileDialog fileDialog = new SaveFileDialog()
                {
                    Filter = "Файл Excel (*.xlsx)|*.xlsx",
                    CheckFileExists = false,
                    AddExtension = true
                };
                if (fileDialog.ShowDialog() == true)
                {
                    if (File.Exists(fileDialog.FileName))
                    {
                        MessageBox.Show("Файл с таким именем уже существует, попробуйте другое имя или измените итоговую папку.", "Файл существует", 
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }
                    bool result = await ExportExcel.Export(path: fileDialog.FileName,
                                        beginDate: BeginDate.SelectedDate.Value,
                                        endDate: EndDate.SelectedDate.Value,
                                        useWastes: (bool)IsWastesCheck.IsChecked);
                    if (result)
                    {
                        MessageBox.Show(
                        "Экспорт данных завершен");
                    }
                }
            }
            else
            {
                MessageBox.Show("Неккоректные данные", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
