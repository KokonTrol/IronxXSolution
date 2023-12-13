using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Threading.Tasks;
using System.Windows;
namespace SysAdminApp
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        Context db = new Context();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            db.HelperInfo.Load();
            HelperList.ItemsSource = db.HelperInfo.Local.ToBindingList();
            db.HelperType.Load();
            TypesList.ItemsSource = db.HelperType.Local.ToBindingList();
        }

        private void AddType_Click(object sender, RoutedEventArgs e)
        {
            db.HelperType.Add(new HelperType() { });
            db.SaveChanges();

        }
        private void AddHelper_Click(object sender, RoutedEventArgs e)
        {
            db.HelperInfo.Add(new HelperInfo() { });
        }

        private void DeleteHelper_Click(object sender, RoutedEventArgs e)
        {
            if (HelperList.SelectedItem != null)
            {
                db.HelperInfo.Remove(HelperList.SelectedItem as HelperInfo);
            }
        }

        private void DeleteType_Click(object sender, RoutedEventArgs e)
        {
            if (TypesList.SelectedItem != null)
            {
                db.HelperType.Remove(TypesList.SelectedItem as HelperType);
                db.SaveChanges();

            }
        }

        private void SaveDBHelper_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                db.SaveChanges();

            }
            catch (Exception ex) {
                MessageBox.Show(ex.ToString(), "Ошибка сохранения", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static JsonSerializerOptions jsonOptions = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true
        };
        private void SaveUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            AskNames askNames = new AskNames();
            if (askNames.ShowDialog() == true)
            {
                var openFileDialog = new OpenFileDialog();
                openFileDialog.CheckFileExists = false;
                openFileDialog.CheckPathExists = true;
                openFileDialog.FileName = "Путь для сохранения";
                openFileDialog.Filter = "Папки|no.files";
                openFileDialog.ValidateNames = false;

                if (openFileDialog.ShowDialog() == true)
                {
                    string folderPath = Path.GetDirectoryName(openFileDialog.FileName);
                    string filePath = Path.Combine(folderPath, "filename.txt");
                    db.SaveChanges();
                    string json = JsonSerializer.Serialize(db.HelperInfo.Local.ToList(), jsonOptions);
                    File.WriteAllText(folderPath + "\\HelperText.json", json);

                    json = JsonSerializer.Serialize(db.HelperType.Local.ToList(), jsonOptions);
                    File.WriteAllText(folderPath + "\\HelperTypes.json", json);
                    File.WriteAllText(folderPath + "\\HelperVer", askNames.HelperVer.Text);
                }
            }
        }

        private async Task Searching(string searchText)
        {
            HelperList.IsEnabled = false;
            await Task<bool>.Run(async () =>
               await FindKeys.GetKeys(searchText)).ContinueWith(t =>
               {
                   Keys.Text = t.Result;
                   HelperList.IsEnabled = true;
               }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        private async void ButtonFinKeys_Click(object sender, RoutedEventArgs e)
        {
            await Searching(HelperText.Text);

        }
    }
}
