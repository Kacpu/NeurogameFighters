using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NeurogameFighters.Views
{
    class BulletView
    {
        public ImageBrush BulletFill { get; }
        private readonly ImageBrush bulletImage = new ImageBrush();

        public BulletView(int jetId)
        {
            SelectImage(jetId);
            BulletFill = bulletImage;
        }

        private void SelectImage(int id)
        {
            if (id == 1)
            {
                bulletImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/15.png"));
            }
            else if (id == 2)
            {
                bulletImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/images/19.png"));
            }
        }
    }
}
