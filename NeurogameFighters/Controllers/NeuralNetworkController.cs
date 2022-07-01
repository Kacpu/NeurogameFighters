using MathNet.Numerics.LinearAlgebra;
using NeurogameFighters.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Threading;

namespace NeurogameFighters.Controllers
{
    class NeuralNetworkController : AIController
    {
        private readonly DispatcherTimer neuralNetworkControllerTimer = new DispatcherTimer();
        private readonly Fighter fighter;
        private readonly Fighter opponent;
        private readonly int maxDecisionsNumber = 5;
        private double firstMaxOutput;
        //private double makeDecisionTreshold = 0.5;
        //int counter = 0;
        private readonly int reactionTime = 100; //ms

        public NeuralNetworkController(Fighter fighter, Fighter opponent)
        {
            this.fighter = fighter;
            this.opponent = opponent;
        }

        public override void StartController()
        {
            //fighter.FightEnd += OnFightEnd;
            neuralNetworkControllerTimer.Tick += Control;
            neuralNetworkControllerTimer.Interval = TimeSpan.FromMilliseconds(reactionTime);
            neuralNetworkControllerTimer.Start();
        }

        private void Control(object sender, EventArgs e)
        {
            if (fighter.Fighting == false)
            {
                neuralNetworkControllerTimer.Stop();
            }

            Vector<double> output = fighter.NeuralNetwork.GetOutput(GetInput());
          /*  counter++;
            if(counter == 300)
            {
                Debug.WriteLine(output);
                counter = 0;
            }*/

            List<int> decisions = new List<int>();

            for (int i = 0; i < maxDecisionsNumber; i++)
            {
                int maxIdxOutput = output.MaximumIndex();

                if(i == 0)
                {
                    firstMaxOutput = output.Maximum();
                }

                MakeDecision(maxIdxOutput, decisions, output);
            }

            for (int i = 0; i < 5; i++)
            {
                if (!decisions.Contains(i))
                {
                    StopAction(i);
                }
            }
        }

        private void MakeDecision(int decision, List<int> decisions, Vector<double> output)
        {
            switch (decision)
            {
                case 0:
                    if (!decisions.Contains(1) && output[0] > 0)
                    {
                        fighter.MovingForward = true;
                        output[0] = double.MinValue;
                        decisions.Add(0);
                    }
                    break;
                case 1:
                    if (!decisions.Contains(0) && output[1] > 0)
                    {
                        fighter.MovingBack = true;
                        output[1] = double.MinValue; ;
                        decisions.Add(1);
                    }
                    break;
                case 2:
                    if (!decisions.Contains(3) && output[2] > 0)
                    {
                        fighter.TurningLeft = true;
                        output[2] = double.MinValue; ;
                        decisions.Add(2);
                    }
                    break;
                case 3:
                    if (!decisions.Contains(2) && output[3] > 0)
                    {
                        fighter.TurningRight = true;
                        output[3] = double.MinValue; ;
                        decisions.Add(3);
                    }
                    break;
                case 4:
                    if (output[4] > 0)
                    {
                        fighter.Shooting = true;
                        output[4] = double.MinValue;
                        decisions.Add(4);
                    }
                    break;
                default:
                    Debug.WriteLine("Error! Decyzja nie istnieje.");
                    break;
            }
        }

        private void StopAction(int action)
        {
            switch (action)
            {
                case 0:
                    fighter.MovingForward = false;
                    break;
                case 1:
                    fighter.MovingBack = false;
                    break;
                case 2:
                    fighter.TurningLeft = false;
                    break;
                case 3:
                    fighter.TurningRight = false;
                    break;
                case 4:
                    fighter.Shooting = false;
                    break;
                default:
                    Debug.WriteLine("Error! Akcja nie istnieje.");
                    break;
            }
        }

        private Vector<double> GetInput()
        {
            Vector<double> input = Vector<double>.Build.Dense(fighter.NeuralNetwork.InputLayerSize);
            Point fighterPoint = fighter.Boundary.GetMiddlePointOfRightSide();
            Point opponentPoint = opponent.Boundary.GetMiddlePointOfRightSide();

            input[0] = fighterPoint.X;
            input[1] = fighterPoint.Y;
            input[2] = fighter.Angle;
            input[3] = opponentPoint.X;
            input[4] = opponentPoint.Y;

            return input;
        }

        /*
        private Vector<double> GetInput()
        {
            Vector<double> input = Vector<double>.Build.Dense(inputSize);
            int parts = (inputSize - 4) / 2;
            int splitAngle = 360 / parts;

            CheckOpponentPosition(input, splitAngle);

            foreach (Bullet bullet in opponent.Bullets)
            {
                CheckBulletPosition(input, bullet, parts, splitAngle);
            }

            CheckWalls(input, parts);

            return input;
        }

        private void CheckOpponentPosition(Vector<double> input, int splitAngle)
        {
            Point fighterPoint = fighter.Boundary.GetMiddlePointOfRightSide();
            Point opponentPoint = opponent.Boundary.GetMiddlePointOfRightSide();
            double angle = GetAngelFromFighterToObject(fighterPoint, opponentPoint);
            input[(int)(angle / splitAngle)] = 1 / GetLengthFromFighter(fighterPoint, opponentPoint);
        }

        private void CheckBulletPosition(Vector<double> input, Bullet bullet, int parts, int splitAngle)
        {
            Point fighterPoint = fighter.Boundary.GetMiddlePointOfRightSide();
            Point bulletPoint = bullet.Boundary.GetMiddlePointOfRightSide();
            double angle = GetAngelFromFighterToObject(fighterPoint, bulletPoint);
            try
            {
                input[(int)(angle / splitAngle) + parts] += 1 / GetLengthFromFighter(fighterPoint, bulletPoint);
            }
            catch (ArgumentOutOfRangeException)
            {
                Debug.WriteLine("idx: " + (int)(angle / 60) + 6 + "kąt: " + angle);
            }
        }

        private void CheckWalls(Vector<double> input, int parts)
        {
            Point fighterPoint = fighter.Boundary.GetMiddlePointOfRightSide();

            input[2 * parts] = 1 / GetLengthFromFighter(fighterPoint, new Point(fighterPoint.X, 0));
            input[2 * parts + 1] = 1 / GetLengthFromFighter(fighterPoint, new Point(1280, fighterPoint.Y));
            input[2 * parts + 2] = 1 / GetLengthFromFighter(fighterPoint, new Point(fighterPoint.X, 720));
            input[2 * parts + 3] = 1 / GetLengthFromFighter(fighterPoint, new Point(0, fighterPoint.Y));
        }

        private double GetLengthFromFighter(Point fighterPoint, Point point)
        {
            return Math.Sqrt(Math.Pow(fighterPoint.X - point.X, 2) + Math.Pow(fighterPoint.Y - point.Y, 2));
        }

        private double GetAngelFromFighterToObject(Point fighterPoint, Point objectPoint)
        {
            Point commonPoint = new Point(fighterPoint.X, objectPoint.Y);

            double diagonal = GetLengthFromFighter(fighterPoint, objectPoint);
            double verticalSide = GetLengthFromFighter(fighterPoint, commonPoint);
            double angle;
            if (diagonal == 0)
            {
                angle = 0;
            }
            else
            {
                angle = Math.Acos(verticalSide / diagonal);
            }
            //Debug.WriteLine(angle);

            angle = angle * 180 / Math.PI;

            if (angle == 90)
            {
                angle = 0;
            }

            if (fighterPoint.X <= objectPoint.X && fighterPoint.Y < objectPoint.Y)
            {
                angle = 180 - angle;
            }
            else if (fighterPoint.X > objectPoint.X && fighterPoint.Y < objectPoint.Y)
            {
                angle = 180 + angle;
            }
            else if (fighterPoint.X >= objectPoint.X && fighterPoint.Y > objectPoint.Y)
            {
                angle = 360 - angle;

                if (angle == 360)
                {
                    angle = 0;
                }
            }
            else if (fighterPoint.X < objectPoint.X && fighterPoint.Y == objectPoint.Y)
            {
                angle += 90;
            }
            else if (fighterPoint.X > objectPoint.X && fighterPoint.Y == objectPoint.Y)
            {
                angle += 270;
            }

            if (angle >= 360 || angle < 0)
            {
                angle = 0;
            }

            return angle;
        }

        private void OnFightEnd()
        {
            neuralNetworkControllerTimer.Stop();
            fighter.FightEnd -= OnFightEnd;
        }
        */
    }
}
