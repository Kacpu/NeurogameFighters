using MathNet.Numerics.LinearAlgebra;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace NeurogameFighters.Models
{
    class NeuralNetwork
    {
        public Matrix<double> InputToFirstHiddenWeights { get; set; }
        public Matrix<double> FirstHiddenToSecondHiddenWeights { get; set; }
        public Matrix<double> SecondHiddenToOutputWeights { get; set; }
        public Vector<double> DNA { get; set; }
        public int DNASize { get; }
        private readonly Random random = new Random();
        public int InputLayerSize { get; }

        public NeuralNetwork(int inputLayerSize, int hiddenLayerSize, int outputLayerSize)
        {
            InputLayerSize = inputLayerSize;

            InputToFirstHiddenWeights = Matrix<double>.Build.Dense(hiddenLayerSize, inputLayerSize + 1);
            FirstHiddenToSecondHiddenWeights = Matrix<double>.Build.Dense(hiddenLayerSize, hiddenLayerSize + 1);
            SecondHiddenToOutputWeights = Matrix<double>.Build.Dense(outputLayerSize, hiddenLayerSize + 1);

            DNASize = InputToFirstHiddenWeights.ColumnCount * InputToFirstHiddenWeights.RowCount +
                FirstHiddenToSecondHiddenWeights.ColumnCount * FirstHiddenToSecondHiddenWeights.RowCount +
                SecondHiddenToOutputWeights.ColumnCount * SecondHiddenToOutputWeights.RowCount;

            DNA = Vector<double>.Build.Dense(DNASize);

            SetRandomDNA();
            DNAToLayers();
        }

        private void SetRandomDNA()
        {
            for (int i = 0; i < DNA.Count; i++)
            {
                DNA[i] = (random.NextDouble() * 2 - 1) * Math.Pow(10, random.Next(-3, 4));
            }
        }

        public Vector<double> GetOutput(Vector<double> inputs)
        {
            inputs = ExtendForBias(inputs);

            Vector<double> firstHiddenNeurons = InputToFirstHiddenWeights * inputs;
            firstHiddenNeurons = Activation(firstHiddenNeurons);
            firstHiddenNeurons = ExtendForBias(firstHiddenNeurons);

            Vector<double> secondHiddenNeurons = FirstHiddenToSecondHiddenWeights * firstHiddenNeurons;
            secondHiddenNeurons = Activation(secondHiddenNeurons);
            secondHiddenNeurons = ExtendForBias(secondHiddenNeurons);

            Vector<double> outputNeurons = SecondHiddenToOutputWeights * secondHiddenNeurons;
            outputNeurons = Activation(outputNeurons);

            return outputNeurons;
        }

        private Vector<double> ExtendForBias(Vector<double> inputs)
        {
            return CreateVector.DenseOfEnumerable(inputs.Append(1));
        }

        private Vector<double> Activation(Vector<double> inputs)
        {
            for (int i = 0; i < inputs.Count; i++)
            {
                inputs[i] = ReLU(inputs[i]);
            }

            return inputs;
        }

        private double Sigmoid(double sum)
        {
            return 1 / (1 + Math.Pow(Math.E, -sum));
        }

        private double ReLU(double sum)
        {
            return sum < 0 ? 0 : sum;
        }

        public void DNAToLayers()
        {
            int dnaCounter = 0;

            for (int i = 0; i < InputToFirstHiddenWeights.RowCount; i++)
            {
                for (int j = 0; j < InputToFirstHiddenWeights.ColumnCount; j++)
                {
                    InputToFirstHiddenWeights[i, j] = DNA[dnaCounter++];
                }
            }

            for (int i = 0; i < FirstHiddenToSecondHiddenWeights.RowCount; i++)
            {
                for (int j = 0; j < FirstHiddenToSecondHiddenWeights.ColumnCount; j++)
                {
                    FirstHiddenToSecondHiddenWeights[i, j] = DNA[dnaCounter++];
                }
            }

            for (int i = 0; i < SecondHiddenToOutputWeights.RowCount; i++)
            {
                for (int j = 0; j < SecondHiddenToOutputWeights.ColumnCount; j++)
                {
                    SecondHiddenToOutputWeights[i, j] = DNA[dnaCounter++];
                }
            }
        }

        public void LayersToDNA()
        {
            int dnaCounter = 0;

            for (int i = 0; i < InputToFirstHiddenWeights.RowCount; i++)
            {
                for (int j = 0; j < InputToFirstHiddenWeights.ColumnCount; j++)
                {
                    DNA[dnaCounter++] = InputToFirstHiddenWeights[i, j];
                }
            }

            for (int i = 0; i < FirstHiddenToSecondHiddenWeights.RowCount; i++)
            {
                for (int j = 0; j < FirstHiddenToSecondHiddenWeights.ColumnCount; j++)
                {
                    DNA[dnaCounter++] = FirstHiddenToSecondHiddenWeights[i, j];
                }
            }

            for (int i = 0; i < SecondHiddenToOutputWeights.RowCount; i++)
            {
                for (int j = 0; j < SecondHiddenToOutputWeights.ColumnCount; j++)
                {
                    DNA[dnaCounter++] = SecondHiddenToOutputWeights[i, j];
                }
            }
        }

        public void WriteDNAToFile(string path)
        {
            LayersToDNA();

            string line = "";

            for (int i = 0; i < DNA.Count; i++)
            {
                line += DNA[i];
                line += ";";
            }

            line += "\n";
            File.WriteAllText(path, line);
        }

        public bool ReadDNAFromFile(string path)
        {
            string[] lines = File.ReadAllLines(path);

            if (lines.Length == 0)
            {
                return false;
            }

            string[] genes = lines[0].Split(';');
            Array.Resize(ref genes, genes.Length - 1);

            if (genes.Length != DNASize)
            {
                return false;
            }

            DNA = Vector<double>.Build.DenseOfEnumerable(genes.Select(gene => Convert.ToDouble(gene)));

            DNAToLayers();
            return true;
        }
    }
}
