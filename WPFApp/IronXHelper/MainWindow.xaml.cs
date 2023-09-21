using Library.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace IronXHelper
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
        #endregion


        private IEnumerable<HelperInfo> GetHelperInfoList()
        {
            IronContext context = new IronContext();
            context.HelperInfo.Load();
            return context.HelperInfo.Local.OrderBy(x => x.Type).ToList();
        }
        public IEnumerable<HelperInfo> HelperInfoList { get; set; }
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;
            HelperInfoList = GetHelperInfoList();
            if (HelperInfoList.Count() == 0)
            {
                UpdateHelper();
            }
        }

        private void BlockUI()
        {
            CheckUpdateButton.IsEnabled = FindHelpsButton.IsEnabled = false;
            Spinner.Visibility = Visibility.Visible;
            HelperInfoList = new List<HelperInfo>();
            OnPropertyChanged("HelperInfoList");
            HelperList.SelectedItem = null;
            Thread.Sleep(10);
        }
        private void UnBlockUI()
        {
            Spinner.Visibility = Visibility.Collapsed;
            CheckUpdateButton.IsEnabled = FindHelpsButton.IsEnabled = true;
        }

        private async Task UpdateHelper()
        {
            BlockUI();
            await Task<bool>.Run(async () =>
               await Downloader.DownloadInfo()).ContinueWith(t =>
            {
                if (!t.Result)
                {
                    MessageBox.Show("Не удалось обновить информацию.");
                }
                UnBlockUI();
                HelperInfoList = GetHelperInfoList();
                OnPropertyChanged("HelperInfoList");
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        private void CheckUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateHelper();
        }

        private void Image_MouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            FullScreenImage fullScreenImage = new FullScreenImage(((Image)sender).Source);
            fullScreenImage.ShowDialog();
        }

        private async Task Searching(string searchLine)
        {
            BlockUI();
            await Task<bool>.Run(async () =>
               await SearchingHelperInfo.Searching(searchLine)).ContinueWith(t =>
               {
                   UnBlockUI();
                   HelperInfoList = t.Result;
                   OnPropertyChanged("HelperInfoList");
               }, TaskScheduler.FromCurrentSynchronizationContext());
        }

        private async void FindHelpsButton_Click(object sender, RoutedEventArgs e)
        {
            BlockUI();
            if (SearchPanelBox.Text == "")
            {
                HelperInfoList = GetHelperInfoList();
                OnPropertyChanged("HelperInfoList");
                UnBlockUI();
            }
            else
            {
                Searching(SearchPanelBox.Text);
            }
        }

        private void SearchPanelBox_KeyUp(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                FindHelpsButton_Click(null, null);
            }
        }
    }
}
