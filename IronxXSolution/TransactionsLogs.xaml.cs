using Library.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace IronxXSolution
{
    public partial class TransactionsLogs : Window, INotifyPropertyChanged
    {
        IronContext db = new IronContext();
        public TransactionsLogs()
        {
            db.Transaction.Load();
            db.Waste.Load();

            InitializeComponent();
            MinSelectedDate.SelectedDate = DateTime.Now.Date;
            MaxSelectedDate.SelectedDate = DateTime.Now.Date;
            UpdateSumm();
            DataContext = this;
        }

        public IEnumerable<Transaction> Transactions
        {
            get
            {
                db.Logs.Load();
                return db.Transaction.Local.Where(l => l.Date.Date <= MaxSelectedDate.SelectedDate.Value.Date && l.Date.Date >= MinSelectedDate.SelectedDate.Value.Date)
                    .OrderByDescending(c => c.Date).ToList();
            }
            set { OnPropertyChanged("Transactions"); }
        }
        public IEnumerable<Waste> Wastes
        {
            get
            {
                db.Logs.Load();
                return db.Waste.Local.Where(l => l.Date.Date <= MaxSelectedDate.SelectedDate.Value.Date && l.Date.Date >= MinSelectedDate.SelectedDate.Value.Date)
                    .OrderByDescending(c => c.Date).ToList();
            }
            set { OnPropertyChanged("Wastes"); }
        }

        private void EndDateCalendarChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MaxSelectedDate.SelectedDate.Value.Date != null)
            {
                if (MinSelectedDate.SelectedDate.Value.Date > MaxSelectedDate.SelectedDate.Value.Date)
                {
                    MinSelectedDate.SelectedDate = MaxSelectedDate.SelectedDate.Value.Date;
                }
            }
        }

        private void UpdateSumm()
        {
            WastesSumm.Text = $"Итого {Wastes.Sum(s => s.Value).ToString()}р.";
            TransactionsSumm.Text = $"Итого {Transactions.Sum(s => s.Paid).ToString()}р.";;
        }
        private void FindLogsByDateCommand(object sender, RoutedEventArgs e)
        {
            OnPropertyChanged("Wastes");
            OnPropertyChanged("Transactions");
            UpdateSumm();
        }

        private void OpenSettingExcel(object sender, RoutedEventArgs e)
        {
            SettingExcel wind = new SettingExcel(MinSelectedDate.SelectedDate.Value.Date, MaxSelectedDate.SelectedDate.Value.Date);
            wind.Owner = this;
            wind.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            wind.ShowDialog();
        }

        #region propertyChangrd
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        #endregion

        
    }
}
