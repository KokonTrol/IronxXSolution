using Library.Models;
using System.Windows;
using Library.Functions;
using System.Windows.Controls;

namespace IronxXSolution
{
    public partial class ShiftConfig : Page
    {
        private Config _config;
        IronContext _ironContext = new IronContext();
        public ShiftConfig()
        {
            _config = _ironContext.GetConfig();
            
            InitializeComponent();
            NightWorkValue.Value = _config.NightWork;
            DayWorkValue.Value = _config.DayWork;
        }

        private async void Save_Changes(object sender, RoutedEventArgs e)
        {
            if (DayWorkValue.Value < NightWorkValue.Value)
            {
                _ironContext.SetNewConfigShift((int)DayWorkValue.Value, (int)NightWorkValue.Value);
            }
            else
            {
                MessageBox.Show("Неккоректные данные", "Предупреждение", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
