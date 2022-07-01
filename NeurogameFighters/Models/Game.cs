using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Threading;

namespace NeurogameFighters.Models
{
    class Game
    {
        private readonly DispatcherTimer gameTimer = new DispatcherTimer();
        private readonly Stopwatch stopwatch = new Stopwatch();
        public long GameTime { get; set; } //s
        private int maxGameTime = 10; //min
        public event Action GameOver;
        private readonly Fighter fighter1;
        private readonly Fighter fighter2;
        private readonly Collisions Collisions;
        private readonly int fighterLife;
        private bool timeLimitedMode = false;

        public Game(Fighter fighter1, Fighter fighter2, bool timeLimitedMode, int fighterLife, int shootIntervalTime)
        {
            this.fighter1 = fighter1;
            this.fighter2 = fighter2;
            this.timeLimitedMode = timeLimitedMode;
            this.fighterLife = fighterLife;
            this.fighter1.SetParameters(fighterLife, shootIntervalTime);
            this.fighter2.SetParameters(fighterLife, shootIntervalTime);
            this.fighter1.FightEnd += OnFightEnd;
            this.fighter2.FightEnd += OnFightEnd;
            Collisions = new Collisions();
        }

        public void StartNewGame()
        {
            gameTimer.Interval = TimeSpan.FromMilliseconds(10);
            gameTimer.Tick += GameLoop;
            gameTimer.Start();
            stopwatch.Start();
            fighter1.Fighting = true;
            fighter2.Fighting = true;
        }

        private void GameLoop(object sender, EventArgs e)
        {
            if (timeLimitedMode)
            {
                CheckGameTime();
            }
            fighter1.Move();
            fighter2.Move();
            GiveFightingStatistics();
            fighter1.Shoot();
            fighter2.Shoot();
            BulletsMove(fighter1.Bullets);
            BulletsMove(fighter2.Bullets);
            Collisions.CheckBulletsCollisions(fighter1, fighter2.Bullets);
            Collisions.CheckBulletsCollisions(fighter2, fighter1.Bullets);
            Collisions.CheckJetWithBoundary(fighter1);
            Collisions.CheckJetWithBoundary(fighter2);
            Collisions.CheckJetWithJet(fighter1, fighter2);
        }

        private void BulletsMove(ObservableCollection<Bullet> Bullets)
        {
            foreach (Bullet bullet in Bullets)
            {
                bullet.Move();
            }
        }

        public void EndGame()
        {
            fighter1.shootTimer.Stop();
            fighter2.shootTimer.Stop();
            gameTimer.Stop();
            stopwatch.Stop();
            GameTime = stopwatch.ElapsedMilliseconds;
            GameTime /= 1000;
            fighter1.FightEnd -= OnFightEnd;
            fighter2.FightEnd -= OnFightEnd;

            GiveStatistics();
        }

        private void OnFightEnd()
        {
            EndGame();
            OnGameOver();
        }

        private void OnGameOver()
        {
            GameOver?.Invoke();
        }

        private void GiveStatistics()
        {
            if (fighter1.Life > fighter2.Life)
            {
                fighter1.Points = 3;
            }
            else if (fighter1.Life < fighter2.Life)
            {
                fighter2.Points = 3;
            }
            else
            {
                fighter1.Points = 1;
                fighter2.Points = 1;
            }

            fighter1.GameTime = (int)GameTime;
            fighter2.GameTime = (int)GameTime;

            fighter1.Shoots = fighterLife - fighter2.Life;
            fighter2.Shoots = fighterLife - fighter1.Life;

            fighter1.Fighting = false;
            fighter2.Fighting = false;
        }

        private void GiveFightingStatistics()
        {
            if(CheckNavigatedAngle(fighter1, fighter2))
            {
                fighter1.NavigatedPoints += 1;
            }

            if (CheckNavigatedAngle(fighter2, fighter1))
            {
                fighter2.NavigatedPoints += 1;
            }
        }

        //oblicza kąt pomiedzy lufą, a przeciwnikiem i sprawdza czy ten kąt jest w polu strzału
        private bool CheckNavigatedAngle(Fighter fighter, Fighter opponent)
        {
            Point f1 = fighter.Boundary.GetMiddlePointOfRightSide();
            Point o = new Point(opponent.Left + opponent.Width / 2, opponent.Top + opponent.Height / 2);
            Point f2 = new(f1.X + Math.Cos(fighter.Angle * Math.PI / 180) * 10, f1.Y + Math.Sin(fighter.Angle * Math.PI / 180) * 10);

            double f1o = GetLengthBetweenPoints(f1, o);
            double f1f2 = GetLengthBetweenPoints(f1, f2);
            double scalar = (f2.X - f1.X) * (o.X - f1.X) + (f2.Y - f1.Y) * (o.Y - f1.Y);

            double cos = scalar / (f1f2 * f1o);

            double angleToOpponent = Math.Acos(cos) * 180 / Math.PI;

            double visibilityAngle = Math.Atan((opponent.Width / 2) / f1o) * 180 / Math.PI;

           /* if(visibilityAngle > 20)
            {
                Debug.WriteLine(angleToOpponent + "  " + visibilityAngle);

            }*/

            return angleToOpponent <= visibilityAngle;
        }

        private double GetLengthBetweenPoints(Point p1, Point p2)
        {
            return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        private void CheckGameTime()
        {
            if (stopwatch.ElapsedMilliseconds > maxGameTime * 60 * 1000)
            {
                OnFightEnd();
            }
        }
    }
}
