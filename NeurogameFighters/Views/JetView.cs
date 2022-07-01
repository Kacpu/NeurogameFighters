using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NeurogameFighters.Views
{
    class JetView
    {
        public ImageBrush JetFill { get; }
        private readonly ImageBrush jetImage = new ImageBrush();

        public JetView(int jetId)
        {
            SelectImage(jetId);
            JetFill = jetImage;
        }

        private void SelectImage(int id)
        {
            if (id == 1)
            {
                jetImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/player2.png"));
            }
            if (id == 2)
            {
                jetImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/player3.png"));
            }
        }
    }
}
