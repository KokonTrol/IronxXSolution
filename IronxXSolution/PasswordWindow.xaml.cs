using Library.Functions;
using Library.Models;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace IronxXSolution
{
    public partial class PasswordWindow : Window
    {
        bool _firstRun;
        public bool _isAdmin;
        Admin __enteringAdmin = null;
        IronContext _ironxXContext = new IronContext();

        public Admin _enteringAdmin { 
            get => __enteringAdmin;
            set
            {
                __enteringAdmin = value;
                if (__enteringAdmin != null)
                {
                    _firstRun = _firstRun || __enteringAdmin.Password == "";
                    SkipPassword.Visibility = Visibility.Collapsed;
                    if (_firstRun)
                    {
                        FistRunLabels.Visibility = RepeatPassword.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        RepeatPassword.Visibility = Visibility.Collapsed;
                    }
                }
            }
        }

        public PasswordWindow()
        {
            BaseFunctions.CreateDocumentsFolder();

            _firstRun = _ironxXContext.GetConfig().SuperUserPassword.Length == 0;
            InitializeComponent();
            if (!_firstRun)
            {
                RepeatPassword.Visibility = FistRunLabels.Visibility = Visibility.Collapsed;
                SkipPassword.Visibility = Visibility.Visible;
            }
            else
            {
                SkipPassword.Visibility =  Visibility.Collapsed;
            }
            FirstPassword.Focus();
        }

        void ErrorMessage(string message)
        {
            MessageText.Visibility = Visibility.Collapsed;
            MessageText.Content = message;
            MessageText.Visibility = Visibility.Visible;
        }
        async Task EnterApp(bool isAdmin)
        {
            MainWindow window = new MainWindow(isAdmin);
            window.Show();
            this.Owner = window;
            this.Close();
        }

        bool CheckFisrtSecondPassword()
        {
            if (FirstPassword.Password != SecondPassword.Password)
            {
                ErrorMessage("Введенные пароли не совпадают");
                AcceptPassword.IsEnabled = SkipPassword.IsEnabled = true;

                return false;
            }
            else if (FirstPassword.Password.Length < 5)
            {
                ErrorMessage("Пароль должен состоять не менее, чем из 5 символов");
                AcceptPassword.IsEnabled = SkipPassword.IsEnabled = true;

                return false;
            }
            return true;
        }
        async Task Accept()
        {
            if (_enteringAdmin == null)
            {
                if (_firstRun)
                {
                    if (CheckFisrtSecondPassword())
                    {
                        PasswordHash.SavePassword(FirstPassword.Password);
                        await EnterApp(true);
                    }
                }
                else
                {
                    if (PasswordHash.CheckPassword(FirstPassword.Password, _ironxXContext.GetConfig().SuperUserPassword))
                    {
                        await EnterApp(true);
                    }
                    else
                    {
                        ErrorMessage("Введен неверный пароль");
                        AcceptPassword.IsEnabled = SkipPassword.IsEnabled = true;
                    }
                }
            }
            else 
            {
                if (_firstRun)
                {
                    if (CheckFisrtSecondPassword())
                    {
                        IronContext ironContext = new IronContext();
                        ironContext.Admin.Load();
                        ironContext.Admin.Local.Where(a => a.Id == _enteringAdmin.Id).First().Password = PasswordHash.GetHashed(FirstPassword.Password);
                        ironContext.SaveChanges();
                        this.DialogResult = true;
                    }
                }
                else
                {
                    if (PasswordHash.CheckPassword(FirstPassword.Password, _enteringAdmin.Password))
                    {
                        this.DialogResult = true;
                    }
                    else
                    {
                        ErrorMessage("Введен неверный пароль");
                        AcceptPassword.IsEnabled = true;
                    }
                }
            }
        }
        private void AcceptPassword_Click(object sender, RoutedEventArgs e)
        {
            Accept();
        }

        private async void SkipPassword_Click(object sender, RoutedEventArgs e)
        {
            await EnterApp(false);
        }

        private void FirstPassword_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                if (_firstRun)
                {
                    SecondPassword.Focus();
                    return;
                }
                else
                {
                    AcceptPassword.IsEnabled = SkipPassword.IsEnabled = false;
                    Accept();
                    return;
                }
            }
        }

        private void SecondPassword_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                AcceptPassword.IsEnabled = SkipPassword.IsEnabled = false;
                Accept();
                return;
            }
        }
    }
}
