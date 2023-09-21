using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace IronXHelper
{
    public partial class FullScreenImage : Window
    {
        public FullScreenImage(ImageSource image)
        {
            InitializeComponent();
            ImageToShow.Source = image;
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            this.Close();
        }

        private void ImageToShow_KeyDown(object sender, KeyEventArgs e)
        {
            this.Close();
        }
    }
}
