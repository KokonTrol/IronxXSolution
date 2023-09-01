using Microsoft.Win32;
using System.Windows;
using Library.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Library.Functions;

namespace IronxXSolution
{
    public partial class SettingExcel : Window
    {
        public SettingExcel()
        {
            IronContext ironContext = new IronContext();
            ironContext.Transaction.Load();
            var minDate = ironContext.Transaction.Min(t => t.Date);
            var maxDate = ironContext.Transaction.Max(t => t.Date);

            InitializeComponent();

            BeginDate.DisplayDateEnd = EndDate.DisplayDateEnd = ironContext.Transaction.Max(t => t.Date).Date;
            BeginDate.DisplayDateStart = EndDate.DisplayDateStart = ironContext.Transaction.Min(t => t.Date).Date;
            BeginDate.SelectedDate = EndDate.SelectedDate = EndDate.DisplayDateEnd;

        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (BeginDate.SelectedDate.Value <= EndDate.SelectedDate.Value)
            {
                SaveFileDialog fileDialog = new SaveFileDialog()
                {
                    Filter = "Файл Excel (*.xlsx)|*.xlsx"
                };
                if (fileDialog.ShowDialog() == true)
                {
                    bool result = await ExportExcel.Export(path: fileDialog.FileName,
                                        beginDate: BeginDate.SelectedDate.Value,
                                        endDate: EndDate.SelectedDate.Value);
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
