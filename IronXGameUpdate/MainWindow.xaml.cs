using IronXGameUpdate.ViewModel;
using Library.Models;
using System.Windows;

namespace IronXGameUpdate
{
    public partial class MainWindow : Window
    {
        public MainWindow(Admin admin, bool isAdmin)
        {
            InitializeComponent();
            DataContext = new ViewModelMain(admin, isAdmin);
        }
    }
}
