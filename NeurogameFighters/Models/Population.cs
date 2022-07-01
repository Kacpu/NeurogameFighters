using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace NeurogameFighters.Models
{
    class Population
    {
        public List<Fighter> Fighters { get; }
        private List<Fighter> newFighters;
        private Random random = new Random();
        public Fighter BestFighter { get; set; }
        public int BestFitness { get; set; } = 0;
        private readonly int id;
        private readonly int fighterAngle;
        private readonly int fighterLeft;
        private readonly int fighterTop;
        private readonly int size;
        private readonly double mutationRate = 0.01;
        private readonly int networkInputSize;
        private readonly int networkOutputSize;

        public Population(int size, int id, int fighterAngle, int fighterLeft, int fighterTop, int networkInputSize, int networkOutputSize)
        {
            this.size = size;
            this.id = id;
            this.fighterAngle = fighterAngle;
            this.fighterLeft = fighterLeft;
            this.fighterTop = fighterTop;
            this.networkInputSize = networkInputSize;
            this.networkOutputSize = networkOutputSize;

            Fighters = new List<Fighter>();
            newFighters = new List<Fighter>();

            for (int i = 0; i < size; i++)
            {
                Fighter fighter = new Fighter(id, random.Next(360), random.Next(100, 1100), random.Next(100, 600));
                fighter.SetNeuralNetwork(networkInputSize, networkOutputSize);
                Fighters.Add(fighter);
            }
        }

        public void Evolution()
        {
            CalculateFitness();
            SetBestFighter();
            Selection();
            Fighters.Clear();
            Fighters.AddRange(newFighters);
            //TakeBestFighter();
        }

        private void Selection()
        {
            double sum = 0;
            newFighters.Clear();

            foreach (Fighter fighter in Fighters)
            {
                sum += fighter.Fitness;
            }

            foreach (Fighter fighter in Fighters)
            {
                fighter.Prob = fighter.Fitness / sum;
            }

            for (int i = 0; i < size; i++)
            {
                Fighter parent1 = SelectOne();
                Fighter parent2 = SelectOne();

                parent1.NeuralNetwork.LayersToDNA();
                parent2.NeuralNetwork.LayersToDNA();

                Fighter child = Crossover(parent1, parent2);
                Mutation(child);

                child.NeuralNetwork.DNAToLayers();

                newFighters.Add(child);
            }
        }

        public void CalculateFitness()
        {
            foreach (Fighter fighter in Fighters)
            {
                fighter.CalculateFitness();
            }
        }

        private Fighter SelectOne()
        {
            double r = random.NextDouble();
            int i = 0;

            while (r > 0)
            {
                r -= Fighters[i].Prob;
                i++;
            }

            i--;
            return Fighters[i];
        }

        private Fighter Crossover(Fighter parent1, Fighter parent2)
        {
            Fighter child = new Fighter(id, random.Next(360), random.Next(100, 1100), random.Next(100, 600));
            child.SetNeuralNetwork(networkInputSize, networkOutputSize);

            int dnaSize;

            if (parent1.NeuralNetwork.DNA.Count == parent2.NeuralNetwork.DNA.Count)
            {
                dnaSize = parent1.NeuralNetwork.DNA.Count;
            }
            else
            {
                return null;
            }

            int rand = random.Next(dnaSize);

            for (int i = 0; i < dnaSize; i++)
            {
                if (i < rand)
                {
                    child.NeuralNetwork.DNA[i] = parent1.NeuralNetwork.DNA[i];
                }
                else
                {
                    child.NeuralNetwork.DNA[i] = parent2.NeuralNetwork.DNA[i];
                }
            }

            return child;
        }

        private void Mutation(Fighter child)
        {
            for (int i = 0; i < child.NeuralNetwork.DNA.Count; i++)
            {
                if (random.NextDouble() < mutationRate)
                {
                    child.NeuralNetwork.DNA[i] += (random.NextDouble() * 2 - 1) * Math.Pow(10, random.Next(-3, 4));
                }
            }
        }

        private void SetBestFighter()
        {
            double maxFitness = -1;
            for (int i = 0; i < size; i++)
            {
                double fitness = Fighters[i].Fitness;
                if (fitness > maxFitness)
                {
                    BestFighter = Fighters[i];
                    maxFitness = fitness;
                }
            }
            BestFitness = (int)BestFighter.Fitness;
            Debug.WriteLine(BestFitness);
        }

        private void TakeBestFighter()
        {
            BestFighter.NeuralNetwork.LayersToDNA();
            Fighters[0].NeuralNetwork.DNA = BestFighter.NeuralNetwork.DNA;
            Fighters[0].NeuralNetwork.DNAToLayers();
        }
    }
}
