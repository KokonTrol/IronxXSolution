using System.Windows;

namespace IronXHelper
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Downloader.DownloadInfo();
        }
    }
}
