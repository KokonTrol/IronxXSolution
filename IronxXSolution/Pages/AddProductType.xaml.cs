using Library.Models;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using System.Windows.Controls;

namespace IronxXSolution
{
    public partial class AddProductType : Page
    {
        IronContext _ironContext;
        public AddProductType()
        {
            InitializeComponent();
            _ironContext = new IronContext();
            _ironContext.ProductType.Load();
            ProductTypeList.ItemsSource = _ironContext.ProductType.Local.ToBindingList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _ironContext.ProductType.Add(new ProductType());
        }
        private void Save_Changes(object sender, RoutedEventArgs e)
        {
            switch (MessageBox.Show(
                "Сохранить изменения товаров?",
                "Сообщение",
                MessageBoxButton.YesNoCancel))
            {
                case MessageBoxResult.Yes:
                    _ironContext.SaveChanges();
                    _ironContext.ChangeTracker.Clear();
                    break;
                case MessageBoxResult.No:
                    _ironContext.ChangeTracker.Clear();
                    break;
            }

        }
    }
}
