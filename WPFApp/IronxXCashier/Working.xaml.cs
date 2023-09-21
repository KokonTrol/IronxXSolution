using Library.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;

namespace IronxXCashier
{
    public partial class Working : Window
    {
        Admin _admin { get; set; }
        IronContext _ironContext = new IronContext();
        double _cashSumm { get; set; }
        double _nonCashSumm { get; set; }
        Config _config;
        bool _isAdmin;
        public Working(Admin admin, bool isAdmin)
        {
            _admin = admin;
            _config = _ironContext.GetConfig();
            InitializeComponent();
            AdminName.Text = _admin.Name;
            _isAdmin = isAdmin;

            if (DateTime.Now <= DateTime.Now.Date.AddHours(_config.NightWork)
                && DateTime.Now >= DateTime.Now.Date.AddHours(_config.DayWork))
            {
                ShiftTime.Text = "Дневная смена";
                this.Title = $"Смена {_admin.Name}: ДЕНЬ";

            }
            else if (DateTime.Now <= DateTime.Now.Date.AddHours(_config.DayWork) || DateTime.Now >= DateTime.Now.Date.AddHours(_config.NightWork))
            {
                ShiftTime.Text = "Ночная смена";
                this.Title = $"Смена {_admin.Name}: НОЧЬ";
            }

            LoadTransactions();
            foreach (Transaction transaction in _ironContext.Transaction.Local.ToList()) {
                TransactionList.Items.Add(transaction);
            }
            
            _ironContext.ProductType.Where(p=>p.IsActive).Load();
            ProductTypeList.ItemsSource = _ironContext.ProductType.Local.ToBindingList();
            ProductTypeList.SelectedIndex = 0;
        }

        private void LoadTransactions()
        {
            _ironContext = new IronContext();
            if ( DateTime.Now <= DateTime.Now.Date.AddHours(_config.NightWork)
                && DateTime.Now >= DateTime.Now.Date.AddHours(_config.DayWork))
            {
                _ironContext.Transaction.Where(t => //t.AdminId == Admin.Id &&
                                            t.Date >= DateTime.Now.Date.AddHours(_config.DayWork)).Load();
            }
            else if (DateTime.Now <= DateTime.Now.Date.AddHours(_config.DayWork))
            {
                _ironContext.Transaction.Where(t =>//t.AdminId == Admin.Id &&
                                             t.Date >= DateTime.Now.Date.AddDays(-1).AddHours(_config.NightWork)
                                            && t.Date <= DateTime.Now.Date.AddHours(_config.DayWork)).Load();
            }
            else if (DateTime.Now >= DateTime.Now.Date.AddHours(_config.NightWork))
            {
                _ironContext.Transaction.Where(t =>//t.AdminId == Admin.Id &&
                                            t.Date >= DateTime.Now.Date.AddHours(_config.NightWork)).Load();
            }
            _cashSumm = _ironContext.Transaction.Local.Where(x => x.IsCash == true).Sum(x => x.Paid);
            _nonCashSumm = _ironContext.Transaction.Local.Where(x => x.IsCash == false).Sum(x => x.Paid);

            SummNonCashValue.Text = $"Безнал: {_nonCashSumm}₽";
            SummCashValue.Text = $"Наличка: {_cashSumm}₽";
        }

        private void SendTransaction_Click(object sender, RoutedEventArgs e)
        {
            double price;
            if (double.TryParse(PaidValue.Text, out price))
            {
                Transaction newTrans = new Transaction(
                    _admin.Id,
                    ((ProductType)ProductTypeList.SelectedItem).Id,
                    price,
                    (bool)IsCashCheck.IsChecked);
                _ironContext.Transaction.Add(newTrans);
                _ironContext.SaveChanges();
                TransactionList.Items.Add(newTrans);
                if ((bool)IsCashCheck.IsChecked)
                {
                    _cashSumm += price;
                }
                else
                {
                    _nonCashSumm += price;
                }
                SummNonCashValue.Text = $"Безнал: {_nonCashSumm}₽";
                SummCashValue.Text = $"Наличка: {_cashSumm}₽";

                TransPanel.Visibility = Visibility.Collapsed;
                SendTransaction.Visibility = Visibility.Collapsed;
                NewTransaction.Visibility = Visibility.Visible;
            }
            else
            {
                MessageBox.Show("Неккоректные данные", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void NewTransaction_Click(object sender, RoutedEventArgs e)
        {
            TransPanel.Visibility = Visibility.Visible;
            SendTransaction.Visibility = Visibility.Visible;
            NewTransaction.Visibility = Visibility.Collapsed;
        }

        private void Button_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            //MainWindow main = new MainWindow(_admin, _isAdmin);
            //main.Width = this.Width;
            //main.Height = this.Height;
            //main.Top = this.Top;
            //main.Left = this.Left;
            //main.Show();
            //this.Owner = main;
            //this.Close();
        }
    }
}
