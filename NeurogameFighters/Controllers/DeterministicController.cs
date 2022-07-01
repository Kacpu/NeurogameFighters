using NeurogameFighters.Controllers;
using NeurogameFighters.Models;
using System;
using System.Diagnostics;
using System.Windows.Threading;

namespace NeurogameFighters.Controllers
{
    class DeterministicController : AIController
    {
        private readonly DispatcherTimer deterministicControllerTimer = new DispatcherTimer();
        private readonly Fighter fighter;
        private readonly Fighter opponent;
        private readonly Random random = new Random();
        private int previousAngle = 0;
        private double previousLeft = 0;
        private double previousTop = 0;
        private readonly double shootingChance = 0.01;
        private bool shootingNow = false;
        private bool turningNow = false;
        private readonly int reactionTime = 10; //ms

        public DeterministicController(Fighter fighter, Fighter opponent)
        {
            this.fighter = fighter;
            this.opponent = opponent;
        }

        public override void StartController()
        {
            SetStartPoint();
            deterministicControllerTimer.Tick += Control;
            deterministicControllerTimer.Interval = TimeSpan.FromMilliseconds(reactionTime);
            deterministicControllerTimer.Start();
        }

        private void SetStartPoint()
        {
            if(random.Next(4) == 0)
            {
                fighter.Left = 150;
                fighter.Top = 100;
                fighter.Angle = 0;
            }
            else if(random.Next(4) == 1)
            {
                fighter.Left = 1050;
                fighter.Top = 100;
                fighter.Angle = 90;
            }
            else if (random.Next(4) == 2)
            {
                fighter.Left = 1050;
                fighter.Top = 450;
                fighter.Angle = 180;
            }
            else if (random.Next(4) == 3)
            {
                fighter.Left = 150;
                fighter.Top = 450;
                fighter.Angle = 270;
            }

            previousLeft = fighter.Left;
            previousTop = fighter.Top;
            previousAngle = fighter.Angle;
            fighter.MovingForward = true;
        }

        private void Control(object sender, EventArgs e)
        {
            if (fighter.Fighting == false)
            {
                deterministicControllerTimer.Stop();
            }

            if (!shootingNow)
            {
                SetPath();
                CheckStopTurning();
            }

            if (!turningNow)
            {
                Shooting();
            }

            if(shootingNow && !turningNow)
            {
                NavigateOpponent();
            }
        }

        private void SetPath()
        {
            if (Math.Abs(fighter.Left - previousLeft) > 900)
            {
                fighter.TurningRight = true;
                previousLeft = fighter.Left;
                turningNow = true;
            }

            if (Math.Abs(fighter.Top - previousTop) > 350)
            {
                fighter.TurningRight = true;
                previousTop = fighter.Top;
                turningNow = true;
            }
        }

        private void CheckStopTurning()
        {
            if (fighter.Angle >= previousAngle + 90)
            {
                fighter.TurningRight = false;
                fighter.Angle = previousAngle + 90;
                previousAngle = fighter.Angle;
                turningNow = false;
            }
            else if (previousAngle == 270 && fighter.Angle > 350)
            {
                fighter.TurningRight = false;
                fighter.Angle = 0;
                previousAngle = fighter.Angle;
                turningNow = false;
            }
        }

        private void Shooting()
        {
            if(random.NextDouble() < shootingChance)
            {
                shootingNow = true;
                fighter.Shooting = true;
                fighter.MovingForward = false;
            }

            if (random.NextDouble() < shootingChance)
            {
                shootingNow = false;
                fighter.Shooting = false;
                fighter.TurningLeft = false;
                fighter.TurningRight = false;
                fighter.Angle = previousAngle;
                fighter.MovingForward = true;
            }
        }

        //wybiera kierunek obracania na pondstawie znaku iloczynu wektorowego
        private void NavigateOpponent()
        {
            double det = GetVectorDet();

            if(det > 0)
            {
                fighter.TurningLeft = false;
                fighter.TurningRight = true;
            }
            else if (det < 0)
            {
                fighter.TurningRight = false;
                fighter.TurningLeft = true;
            }
            else
            {
                fighter.TurningLeft = false;
                fighter.TurningRight = false;
            }
        }

        //oblicza wyznacznik wektorów do określenia, po której połowie prostej f1f2 leży punkt o, po lewej - ujemny, po prawej - dodatni
        private double GetVectorDet()
        {
            Point f1 = new Point(fighter.Left + fighter.Width / 2, fighter.Top + fighter.Height / 2);
            Point o = new Point(opponent.Left + opponent.Width / 2, opponent.Top + opponent.Height / 2);
            Point f2 = new Point(f1.X + Math.Cos(fighter.Angle * Math.PI / 180) * 10, f1.Y + Math.Sin(fighter.Angle * Math.PI / 180) * 10);

            return (f2.X - f1.X) * (o.Y - f1.Y) - (o.X - f1.X) * (f2.Y - f1.Y);
        }

        /*
         //oblicza kąt od prawej poziomej osi
        private double GetAngelToOpponent()
        {
            Point fighterPoint = fighter.Boundary.GetMiddlePointOfRightSide();
            Point opponentPoint = opponent.Boundary.GetMiddlePointOfRightSide();
            Point commonPoint = new Point(opponentPoint.X, fighterPoint.Y);

            double diagonal = GetLengthFromFighter(fighterPoint, opponentPoint);
            double horizontalSide = GetLengthFromFighter(fighterPoint, commonPoint);
            double angle = diagonal == 0 ? 0 : Math.Acos(horizontalSide / diagonal);

            angle = angle * 180 / Math.PI;

            if (fighterPoint.X > opponentPoint.X && fighterPoint.Y <= opponentPoint.Y)
            {
                angle = 180 - angle;
            }
            else if (fighterPoint.X >= opponentPoint.X && fighterPoint.Y > opponentPoint.Y)
            {
                angle = 180 + angle;
            }
            else if (fighterPoint.X < opponentPoint.X && fighterPoint.Y > opponentPoint.Y)
            {
                angle = 360 - angle;
            }

            return angle;
        }

        private double GetLengthFromFighter(Point fighterPoint, Point point)
        {
            return Math.Sqrt(Math.Pow(fighterPoint.X - point.X, 2) + Math.Pow(fighterPoint.Y - point.Y, 2));
        }
         */
    }
}
