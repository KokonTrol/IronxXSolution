using System;
using System.Windows;
using Library.Models;

namespace IronxXSolution
{
    public partial class AddNewWaste : Window
    {
        public AddNewWaste()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            double paid;
            if (Double.TryParse(newPaidBox.Text, out paid))
            {
                IronContext db = new IronContext();
                db.Waste.Add(new Waste(paid, newNameBox.Text));
                db.SaveChanges();
                this.DialogResult = true;
            }
            else
            {
                MessageBox.Show("Неверное значение стоимости, проверьте написание.", "Ошибка ввода данных");
            }
        }
    }
}
