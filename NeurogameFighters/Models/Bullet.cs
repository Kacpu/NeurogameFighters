using NeurogameFighters.Views;
using NeurogameFighters.Controllers;
using System;

namespace NeurogameFighters.Models
{
    class Bullet : ControllerBase
    {
        public int Height { get; }
        public int Width { get; }
        public double Left { get; set; }
        public double Top { get; set; }
        public int Angle { get; set; }
        private readonly int speed;
        private double xc = 0;
        private double yc = 0;
        public Boundary Boundary;
        public BulletView BulletView { get; }

        public Bullet(double middleLeft, double middleTop, int angle, int jetID)
        {
            Height = 10;
            Width = 20;
            speed = 7;
            Angle = angle;
            GetBoundary(middleLeft, middleTop);
            Left = Boundary.LeftUpper.X;
            Top = Boundary.LeftUpper.Y;
            BulletView = new BulletView(jetID);
        }

        private void BulletLoop(object sender, EventArgs e)
        {
            Move();
        }

        private void GetBoundary(double middleLeft, double middleTop)
        {
            double radians = Boundary.AngleToFirstQuarter(Angle) * Math.PI / 180;

            if (Angle >= 0 && Angle <= 90)
            {
                xc = middleLeft + Math.Cos(radians) * Width / 2;
                yc = middleTop + Math.Sin(radians) * Width / 2;
            }
            else if (Angle > 90 && Angle <= 180)
            {
                xc = middleLeft - Math.Cos(radians) * Width / 2;
                yc = middleTop + Math.Sin(radians) * Width / 2;
            }
            else if (Angle > 180 && Angle <= 270)
            {
                xc = middleLeft - Math.Cos(radians) * Width / 2;
                yc = middleTop - Math.Sin(radians) * Width / 2;
            }
            else if (Angle > 270 && Angle <= 360)
            {
                xc = middleLeft + Math.Cos(radians) * Width / 2;
                yc = middleTop - Math.Sin(radians) * Width / 2;
            }

            Boundary = new Boundary(xc, yc, Height, Width, Angle);
        }

        public void Move()
        {
            Top += speed * Math.Sin(Angle * Math.PI / 180);
            Left += speed * Math.Cos(Angle * Math.PI / 180);
            yc += speed * Math.Sin(Angle * Math.PI / 180);
            xc += speed * Math.Cos(Angle * Math.PI / 180);

            Boundary.BoundaryUpdate(xc, yc, Angle);
            OnPropertyChanged(nameof(Top));
            OnPropertyChanged(nameof(Left));
        }


    }
}
