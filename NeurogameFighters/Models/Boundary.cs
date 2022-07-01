using System;
using System.Collections.Generic;

namespace NeurogameFighters.Models
{
    class Boundary
    {
        public Point LeftUpper { get; set; } = new Point();
        public Point RightUpper { get; set; } = new Point();
        public Point LeftDown { get; set; } = new Point();
        public Point RightDown { get; set; } = new Point();
        public List<Point> Points { get; set; } = new List<Point>();
        private double left;
        private double top;
        private readonly int height;
        private readonly int width;

        public Boundary(double xc, double yc, int height, int width, int angle)
        {
            this.height = height;
            this.width = width;

            BoundaryUpdate(xc, yc, angle);

            Points.Add(LeftUpper);
            Points.Add(RightUpper);
            Points.Add(RightDown);
            Points.Add(LeftDown);
        }

        public void BoundaryUpdate(double xc, double yc, int angle)
        {
            double radians = AngleToFirstQuarter(angle) * Math.PI / 180;
            double newWidth = height * Math.Sin(radians) + width * Math.Cos(radians);
            double newHeight = height * Math.Cos(radians) + width * Math.Sin(radians);

            left = xc - newWidth / 2;
            top = yc - newHeight / 2;

            if (angle >= 0 && angle <= 90)
            {
                LeftUpper.Update(left + height * Math.Sin(radians), top);
                LeftDown.Update(left, top + height * Math.Cos(radians));
                RightUpper.Update(left + newWidth, top + width * Math.Sin(radians));
                RightDown.Update(left + width * Math.Cos(radians), top + newHeight);
            }
            else if (angle > 90 && angle <= 180)
            {
                LeftUpper.Update(left + newWidth, top + height * Math.Cos(radians));
                LeftDown.Update(left + width * Math.Cos(radians), top);
                RightUpper.Update(left + height * Math.Sin(radians), top + newHeight);
                RightDown.Update(left, top + width * Math.Sin(radians));
            }
            else if (angle > 180 && angle <= 270)
            {
                LeftUpper.Update(left + width * Math.Cos(radians), top + newHeight);
                LeftDown.Update(left + newWidth, top + width * Math.Sin(radians));
                RightUpper.Update(left, top + height * Math.Cos(radians));
                RightDown.Update(left + height * Math.Sin(radians), top);
            }
            else if (angle > 270 && angle <= 360)
            {
                LeftUpper.Update(left, top + width * Math.Sin(radians));
                LeftDown.Update(left + height * Math.Sin(radians), top + newHeight);
                RightUpper.Update(left + width * Math.Cos(radians), top);
                RightDown.Update(left + newWidth, top + height * Math.Cos(radians));
            }
        }

        public Point GetMiddlePointOfRightSide()
        {
            return new Point((RightUpper.X + RightDown.X) / 2, (RightUpper.Y + RightDown.Y) / 2);
        }

        public static int AngleToFirstQuarter(int angle)
        {
            if (angle > 90 && angle <= 180)
            {
                angle = 180 - angle;
            }
            else if (angle > 180 && angle <= 270)
            {
                angle -= 180;
            }
            else if (angle > 270 && angle <= 360)
            {
                angle = 360 - angle; ;
            }

            return angle;
        }
    }

    class Point
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Point() { }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }

        public void Update(double x, double y)
        {
            X = x;
            Y = y;
        }
    }
}
