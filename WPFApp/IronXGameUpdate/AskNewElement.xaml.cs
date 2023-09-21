using System.Windows;

namespace IronXGameUpdate
{
    public partial class AskNewElement : Window
    {
        public string newName { get { return newNameBox.Text; }}
        public AskNewElement()
        {
            InitializeComponent();
            newNameBox.Text = "Новое значение";
            newNameBox.SelectAll();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }
    }
}
