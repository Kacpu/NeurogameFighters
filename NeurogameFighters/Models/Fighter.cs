using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Threading;

namespace NeurogameFighters.Models
{
    class Fighter
    {
        public event Action PropertyChanged;
        public event Action FightEnd;
        public int Id { get; }
        public int Life { get; set; }
        public int Points { get; set; } = 0;
        public int Shoots { get; set; } = 0;
        public double NavigatedPoints { get; set; } = 0;
        public int GameTime { get; set; } = 0;
        public double Fitness { get; set; } = 0;
        public double Prob { get; set; }
        public int Height { get; }
        public int Width { get; }
        public double Top { get; set; }
        public double Left { get; set; }
        public int Angle { get; set; }
        public int Speed { get; }
        private readonly int angleSpeed;
        public bool MovingForward { get; set; }
        public bool MovingBack { get; set; }
        public bool TurningLeft { get; set; }
        public bool TurningRight { get; set; }
        public bool Shooting { get; set; }
        private int shootIntervalTime;
        public bool Fighting { get; set; } = false;
        private readonly Stopwatch stopwatch = new Stopwatch();
        private bool shootStarted = false;
        public readonly DispatcherTimer shootTimer = new DispatcherTimer();
        public Boundary Boundary { get; set; }
        public ObservableCollection<Bullet> Bullets { get; set; }
        public NeuralNetwork NeuralNetwork { get; set; }

        public Fighter(int id, int angle, int left, int top)
        {
            Id = id;
            Angle = angle;
            Left = left;
            Top = top;
            Speed = 3;
            angleSpeed = 3;
            Height = 50;
            Width = 50;

            Bullets = new ObservableCollection<Bullet>();
            Boundary = new Boundary(left + Width / 2, top + Height / 2, Height, Width, angle);

            shootTimer.Tick += OnShoot;
        }

        public void SetParameters(int life, int shootIntervalTime)
        {
            Life = life;
            this.shootIntervalTime = shootIntervalTime;
        }

        public void SetNeuralNetwork(int inputSize, int outputSize)
        {
            NeuralNetwork = new NeuralNetwork(inputSize, 2 * inputSize, outputSize);
        }

        public void Move()
        {
            if (MovingForward)
            {
                MoveForward();
            }
            if (MovingBack)
            {
                MoveBack();
            }
            if (TurningLeft)
            {
                TurnLeft();
            }
            if (TurningRight)
            {
                TurnRight();
            }
        }

        public void MoveForward()
        {
            Top += Speed * Math.Sin(Angle * Math.PI / 180);
            Left += Speed * Math.Cos(Angle * Math.PI / 180);
            UpdatePosittion();
        }

        public void MoveBack()
        {
            Top -= Speed * Math.Sin(Angle * Math.PI / 180);
            Left -= Speed * Math.Cos(Angle * Math.PI / 180);
            UpdatePosittion();
        }

        public void TurnLeft()
        {
            Angle -= angleSpeed;

            if (Angle < 0)
            {
                Angle = 360 + Angle;
            }

            UpdatePosittion();
        }

        public void TurnRight()
        {
            Angle += angleSpeed;

            if (Angle > 360)
            {
                Angle -= 360;
            }

            UpdatePosittion();
        }

        public void Shoot()
        {
            if (Shooting && !shootStarted)
            {
                stopwatch.Stop();
                long breakTime = stopwatch.ElapsedMilliseconds > shootIntervalTime || stopwatch.ElapsedMilliseconds == 0 ?
                    100 : shootIntervalTime - stopwatch.ElapsedMilliseconds;
                shootTimer.Interval = TimeSpan.FromMilliseconds(breakTime);
                shootTimer.Start();
                shootStarted = true;
                stopwatch.Reset();
            }
        }

        private void OnShoot(object sender, EventArgs e)
        {
            shootTimer.Interval = TimeSpan.FromMilliseconds(shootIntervalTime);
            Point middlePoint = Boundary.GetMiddlePointOfRightSide();
            Bullets.Add(new Bullet(middlePoint.X, middlePoint.Y, Angle, Id));

            if (!Shooting && shootStarted)
            {
                shootTimer.Stop();
                stopwatch.Start();
                shootStarted = false;
            }
        }

        public void Hit()
        {
            Life--;
            OnPropertyChanged();

            if (Life <= 0)
            {
                OnFightEnd();
            }
        }

        public void CalculateFitness()
        {
            //Fitness = 30 * Points + 20 * Shoots + 10 * Life + 10 * (60 / GameTime);

            if(Points != 3)
            {
                Points = Shoots * 500;
            }
            else
            {
                Points *= 1000;
            }

            if (NavigatedPoints <= 600)
            {
                Fitness = Points + NavigatedPoints * 10 + 50 * Life;
            }
            else
            {
                Fitness = Points + NavigatedPoints + 5400 + 50 * Life;
            }
                
            Fitness = Math.Pow(Fitness, 2) / 10000;
            Debug.WriteLine(NavigatedPoints);
        }

        private void OnFightEnd()
        {
            FightEnd?.Invoke();
        }

        public void UpdatePosittion()
        {
            Boundary.BoundaryUpdate(Left + Width / 2, Top + Height / 2, Angle);
            OnPropertyChanged();
        }

        private void OnPropertyChanged()
        {
            PropertyChanged?.Invoke();
        }
    }
}
