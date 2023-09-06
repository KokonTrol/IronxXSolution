using Library.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace IronxXSolution.Pages
{
    public partial class AdminSettings : Page
    {
        IronContext _ironContext;
        public AdminSettings()
        {
            InitializeComponent();
            _ironContext = new IronContext();
            _ironContext.Admin.Load();
            AdminsList.ItemsSource = _ironContext.Admin.Local.ToBindingList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _ironContext.Admin.Add(new Admin());
        }

        private void Save_Changes(object sender, RoutedEventArgs e)
        {
            switch (MessageBox.Show(
                "Сохранить изменения администраторов?",
                "Сообщение",
                MessageBoxButton.YesNoCancel))
            {
                case MessageBoxResult.Yes:
                    foreach (var admin in _ironContext.Admin)
                    {
                        admin.Name = admin.Name.Replace(" ", "_");
                    }
                    _ironContext.SaveChanges();
                    _ironContext.ChangeTracker.Clear();

                    break;
                case MessageBoxResult.No:
                    _ironContext.ChangeTracker.Clear();
                    break;
            }

        }

        private void ResetPassword(object sender, RoutedEventArgs e)
        {
            int id;
            if (Int32.TryParse(((Button)sender).Tag.ToString(), out id))
            {
                MessageBoxResult result = MessageBox.Show(
                    "Вы уверены, что хотите сбросить пароль?",
                    "Сообщение",
                    MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    if (_ironContext.Admin.Where(a => a.Id == id).Count() > 0)
                    {
                        _ironContext.Admin.Where(a => a.Id == id).First().Password = "";
                    }
                    else
                    {
                        MessageBox.Show("Неверные данные", "Ошибка");
                    }
                }
            }
        }
    }
}
