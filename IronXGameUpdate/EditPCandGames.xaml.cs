using IronXGameUpdate.ViewModel;
using System.Windows;

namespace IronXGameUpdate
{
    public partial class EditPCandGames : Window
    {
        public EditPCandGames(ViewModelMain parent)
        {
            InitializeComponent();
            var vm = new ViewModelEdit(parent);
            DataContext = vm;

            Closing += vm.OnWindowClosing;
        }
    }
}
