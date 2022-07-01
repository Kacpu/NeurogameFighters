using NeurogameFighters.Controllers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace NeurogameFighters.Models
{
    class BattleCamp
    {
        public event Action PropertyChanged;
        public int GenerationNumber { get; set; } = 1;
        public int FightingPair { get; set; } = 0;
        public int ActiveFights { get; set; }
        public Population Population1 { get; }
        //public Population Population2 { get; }
        public List<Fighter> DeterministicFighters { get; } = new List<Fighter>();
        private readonly int populationSize;
        private readonly int fighterLife;
        private readonly int shootIntervalTime;
        private int fightsCounter = 0;
        private readonly List<Game> battles = new List<Game>();
        public Fighter BestFighter { get; set; }
        public int BestFitness { get; set; } = 0;
        private readonly bool timeLimitedMode = true;

        public BattleCamp(int populationSize, int networkInputSize, int networkOutputSize, int fighterLife, int shootIntervalTime)
        {
            this.populationSize = populationSize;
            this.fighterLife = fighterLife;
            this.shootIntervalTime = shootIntervalTime;

            Population1 = new Population(populationSize, 1, 120, 800, 400, networkInputSize, networkOutputSize);
            //Population2 = new Population(populationSize, 2, 30, 800, 500, networkInputSize, networkOutputSize);
            SetDeterministics();
        }

        public void StartTraining()
        {
            SetBattlefield();
        }

        private void SetBattlefield()
        {
            battles.Clear();
            for (int i = 0; i < populationSize; i++)
            {
                //Game game = new Game(Population1.Fighters[i], Population2.Fighters[i], fighterLife, shootIntervalTime);
                Game game = new Game(Population1.Fighters[i], DeterministicFighters[i], timeLimitedMode ,fighterLife, shootIntervalTime);
                game.GameOver += OnGameOver;
                battles.Add(game);

                //NeuralNetworkController neuralNetworkController1 = new NeuralNetworkController(Population1.Fighters[i], Population2.Fighters[i]);
                NeuralNetworkController neuralNetworkController1 = new NeuralNetworkController(Population1.Fighters[i], DeterministicFighters[i]);
                //NeuralNetworkController neuralNetworkController2 = new NeuralNetworkController(Population2.Fighters[i], Population1.Fighters[i]);
                DeterministicController deterministicController = new DeterministicController(DeterministicFighters[i], Population1.Fighters[i]);

                game.StartNewGame();
                neuralNetworkController1.StartController();
                //neuralNetworkController2.StartController();
                deterministicController.StartController();
            }

            ActiveFights = populationSize;
        }

        public void SetDeterministics()
        {
            DeterministicFighters.Clear();

            for(int i = 0; i< populationSize; i++)
            {
                DeterministicFighters.Add(new Fighter(2, 0, 150, 50));
            }
        }

        private void NextGeneration()
        {
            Population1.Evolution();
            SetBestFighter();
            //Population2.Evolution();
            SetDeterministics();
            GenerationNumber++;
            SetBattlefield();
            OnPropertyChanged();
        }

        public void SelectNextFightingPair()
        {
            for (int i = 0; i < populationSize; i++)
            {
                //if (Population1.Fighters[i].Fighting && Population2.Fighters[i].Fighting)
                if (Population1.Fighters[i].Fighting)
                {
                    FightingPair = i;
                    break;
                }
            }
        }
        
        private void SetBestFighter()
        {
            if (Population1.BestFitness >= BestFitness)
            {
                BestFighter = Population1.BestFighter;
                BestFitness = (int)BestFighter.Fitness;
            }
        }

        private void OnGameOver()
        {
            fightsCounter++;
            ActiveFights--;

            if (fightsCounter == populationSize)
            {
                fightsCounter = 0;
                NextGeneration();
            }

            SelectNextFightingPair();
            OnPropertyChanged();
        }

        public void EndTraining()
        {
            foreach (Game game in battles)
            {
                game.EndGame();
            }

            SaveBestToFile();
        }

        private void OnPropertyChanged()
        {
            PropertyChanged?.Invoke();
        }

        public void SaveBestToFile()
        {
            if (BestFighter == null)
            {
                return;
            }

            string directoryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Neurogame");
            string bestFightersPath = Path.Combine(directoryPath, "bestFighters.csv");

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            if (!File.Exists(bestFightersPath))
            {
                File.Create(bestFightersPath).Close();
            }

            BestFighter.NeuralNetwork.WriteDNAToFile(bestFightersPath);
        }
    }
}
