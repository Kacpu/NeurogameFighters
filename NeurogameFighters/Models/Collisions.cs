using System;
using System.Collections.ObjectModel;

namespace NeurogameFighters.Models
{
    class Collisions
    {
        private readonly int minX = 0;
        private readonly int maxX = 1265;
        private readonly int minY = 0;
        private readonly int maxY = 620;

        public void CheckBulletsCollisions(Fighter jet, ObservableCollection<Bullet> Bullets)
        {
            for (int i = Bullets.Count - 1; i >= 0; i--)
            {
                if (CheckBulletWithJetCollison(jet, Bullets[i]))
                {
                    Bullets.RemoveAt(i);
                    jet.Hit();
                }
                else if (CheckBulletWithBoundary(Bullets[i].Boundary.LeftUpper) && CheckBulletWithBoundary(Bullets[i].Boundary.LeftDown))
                {
                    Bullets.RemoveAt(i);
                }
            }
        }

        private bool CheckBulletWithJetCollison(Fighter jet, Bullet bullet)
        {
            bool collison1 = true;
            bool collison2 = true;
            Point b1 = bullet.Boundary.RightUpper;
            Point b2 = bullet.Boundary.RightDown;

            for (int i = 0; i < 4; i++)
            {
                Point j1 = jet.Boundary.Points[i];
                Point j2 = jet.Boundary.Points[(i + 1) % 4];

                double det1 = (j2.X - j1.X) * (b1.Y - j1.Y) - (b1.X - j1.X) * (j2.Y - j1.Y);
                double det2 = (j2.X - j1.X) * (b2.Y - j1.Y) - (b2.X - j1.X) * (j2.Y - j1.Y);

                collison1 = collison1 && det1 >= 0;
                collison2 = collison2 && det2 >= 0;
            }

            return collison1 || collison2;
        }

        private bool CheckBulletWithBoundary(Point b)
        {
            if (b.X < minX || b.X > maxX || b.Y < minY || b.Y > maxY)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void CheckJetWithBoundary(Fighter jet)
        {
            if (jet.Left < minX)
            {
                jet.Left = 0;
            }
            if (jet.Left + jet.Width > maxX)
            {
                jet.Left = maxX - jet.Width;
            }
            if (jet.Top < minY)
            {
                jet.Top = 0;
            }
            if (jet.Top + jet.Height > maxY)
            {
                jet.Top = maxY - jet.Height;
            }
        }

        public void CheckJetWithJet(Fighter jet1, Fighter jet2)
        {
            if (jet1.Left < jet2.Left + jet2.Width && Math.Abs(jet1.Left + jet1.Width - jet2.Left) <= 3
                && jet1.Top + jet1.Height > jet2.Top && jet1.Top < jet2.Top + jet2.Height)
            {
                jet1.Left -= jet1.Speed + 1;
                jet2.Left += jet2.Speed + 1;
            }

            if (Math.Abs(jet1.Left - (jet2.Left + jet2.Width)) <= 3 && jet1.Left + jet1.Width > jet2.Left
                && jet1.Top + jet1.Height > jet2.Top && jet1.Top < jet2.Top + jet2.Height)
            {
                jet1.Left += jet1.Speed + 1;
                jet2.Left -= jet2.Speed + 1;
            }

            if (jet1.Left < jet2.Left + jet2.Width && jet1.Left + jet1.Width > jet2.Left
                 && Math.Abs(jet1.Top + jet1.Height - jet2.Top) <= 3 && jet1.Top < jet2.Top + jet2.Height)
            {
                jet1.Top -= jet1.Speed + 1;
                jet2.Top += jet2.Speed + 1;
            }

            if (jet1.Left < jet2.Left + jet2.Width && jet1.Left + jet1.Width > jet2.Left
                && jet1.Top + jet1.Height > jet2.Top && Math.Abs(jet1.Top - (jet2.Top + jet2.Height)) <= 3)
            {
                jet1.Top += jet1.Speed + 1;
                jet2.Top -= jet2.Speed + 1;
            }

            jet1.UpdatePosittion();
            jet2.UpdatePosittion();
        }
    }
}
