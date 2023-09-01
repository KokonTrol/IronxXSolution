﻿using Library.Functions;
using Library.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;

namespace IronxXSolution
{
    public partial class MainWindow : Window
    {
        IronContext _ironContext = new IronContext();
        bool _isAdmin;
        Admin _admin;
        public MainWindow(bool isAdmin)
        {
            InitializeComponent();
            _isAdmin = isAdmin;
            if (!_isAdmin)
            {
                AdminsProperties.Visibility = Visibility.Collapsed;
            }
            ListSource();
        }

        void ListSource()
        {
            _ironContext = new IronContext();
            _ironContext.Transaction.Load();
            _ironContext.Admin.Where(a=>a.IsActive).Load();

            if (_ironContext.Admin.Count() == 0)
            {
                HasAdminsBlock.Visibility = Visibility.Collapsed;
                HasNotAdminsBlock.Visibility = Visibility.Visible;

            }
            else
            {
                HasAdminsBlock.Visibility = Visibility.Visible;
                HasNotAdminsBlock.Visibility = Visibility.Collapsed;
                chooseAdminList.ItemsSource = _ironContext.Admin.Local.ToBindingList();
                if (chooseAdminList.Items.Count > 0)
                {
                    chooseAdminList.SelectedItem = _admin;
                }
            }


            ExportExcelMenu.IsEnabled = _ironContext.Transaction.Local.Count != 0;
        }

        private void EnterButton_Click(object sender, RoutedEventArgs e)
        {
            PasswordWindow window = new PasswordWindow();
            window._isAdmin = _isAdmin;
            window._enteringAdmin = chooseAdminList.SelectedItem as Admin;
            if (window.ShowDialog() == true)
            {
                _admin = _ironContext.Admin.First(x => x.Id == window._enteringAdmin.Id);

                OpenAppPanel.Visibility = Visibility.Visible;
                EnterButton.Visibility = Visibility.Collapsed;

                //ListSource();
            }
        }

        private async void MenuItem_Click_2(object sender, RoutedEventArgs e)
        {
            SettingExcel wind = new SettingExcel();
            wind.Owner = this;
            wind.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            wind.ShowDialog();
        }

        private void CashierAppName_Click(object sender, RoutedEventArgs e)
        {
            var data = JsonDocument.Parse(BaseFunctions.GetStringFileFromResources("DBConfig.json"));
            string tag = ((Button)sender).Tag.ToString();
            string file = data.RootElement.GetProperty(tag + "Name").ToString();

            if (System.IO.File.Exists(file))
            {
                System.Diagnostics.Process.Start(file, _admin.ToCommandParameters() + $"-SU {_isAdmin}");
            }
            else MessageBox.Show(data.RootElement.GetProperty(tag + "Error").ToString());
        }

        private void chooseAdminList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            OpenAppPanel.Visibility = Visibility.Collapsed;
            _admin = chooseAdminList.SelectedItem as Admin;
            EnterButton.Visibility = Visibility.Visible;
        }

        private void OpenSettings(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings();
            settings.ShowDialog();
            ListSource();
        }
    }
}
