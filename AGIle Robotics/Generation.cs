using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AGIle_Robotics.Interfaces;

namespace AGIle_Robotics
{
    public class Generation : IGeneration
    {
        public IPopulation[] Populations { get => populations; set => populations = value; }
        private IPopulation[] populations;

        public int Level { get => level; private set => level = value; }
        private int level;

        public int MaxThreads { get => maxThreads; set => maxThreads = value; }
        private int maxThreads = 4;

        public double TransitionRatio { get => transitionRatio; set => transitionRatio = value; }
        private double transitionRatio = 0.5;

        public double RandomRatio { get => randomRatio; set => randomRatio = value; }
        private double randomRatio = 0.1;

        public double MutationRatio { get => mutationRatio; set => mutationRatio = value; }
        private double mutationRatio = 0.1;

        public INeuralNetwork Best { get => best; private set => best = value; }
        public INeuralNetwork best;

        public (double, double) WeightRange { get => weightRange; private set => weightRange = value; }
        private (double, double) weightRange = (-2, 2);

        public Func<double, double> ActivationFunction { get => activationFunction; private set => activationFunction = value; }
        private Func<double, double> activationFunction = Math.Tanh;

        public int MinLength { get => minLength; private set => minLength = value; }
        public int minLength = 5;

        public int MaxLength { get => maxLength; private set => maxLength = value; }
        public int maxLength = 10;

        public int MinWidth { get => minWidth; private set => minWidth = value; }
        public int minWidth = 5;

        public int MaxWidth { get => maxWidth; private set => maxWidth = value; }
        public int maxWidth = 10;

        public Generation((int, int) length, (int, int) width, (double, double) weightRange)
            => Init(length, width, weightRange, Math.Tanh);
        public Generation((int, int) length, (int, int) width, (double, double) weightRange, Func<double, double> activateWith)
            => Init(length, width, weightRange, activateWith);
        private void Init((int, int) length, (int, int) width, (double, double) weightRange, Func<double, double> activateWith)
        {
            MinLength = length.Item1;
            MaxLength = length.Item2;
            MinWidth = width.Item1;
            MaxWidth = width.Item2;

            WeightRange = weightRange;

            ActivationFunction = activateWith;
        }

        public void Evaluate(Func<(double, double), (INeuralNetwork, INeuralNetwork)> fitnessFunction)
        {
            throw new NotImplementedException();
        }

        public IGeneration Evolve()
        {
            return (IGeneration)Evolve(TransitionRatio, RandomRatio, MutationRatio);
        }

        public IEvolvable Evolve(double transitionRatio, double randomRatio, double mutationRatio)
        {
            IGeneration newGen = this;
            List<IPopulation>[] populations = new List<IPopulation>[MaxThreads];
            for(int i = 0; i < newGen.Populations.Length; i++)
            {
                int slot = i % MaxThreads;
                populations[slot].Add(newGen.Populations[i]);
            }

            List<Task<List<IPopulation>>> tasks = new List<Task<List<IPopulation>>>();
            for(int i = 0; i < populations.Length; i++)
            {
                var temp = populations[i];
                tasks.Add(Task<List<IPopulation>>.Factory.StartNew(
                    () => EvolvePopulations(temp, transitionRatio, randomRatio, mutationRatio)));
            }

            Task.WaitAll(tasks.ToArray());

            int count = 0;
            foreach(var task in tasks)
            {
                var list = task.Result;
                for(int i = 0; i < list.Count; i++)
                {
                    newGen.Populations[count] = list[i];
                    count++;
                }
            }
            return newGen;
        }

        private List<IPopulation> EvolvePopulations(List<IPopulation> populations, double transition, double random, double mutation)
        {
            List<IPopulation> newPopulations = new List<IPopulation>();
            for(int i = 0; i < populations.Count; i++)
            {
                var newPop = (IPopulation)populations[i].Evolve(transition, random, mutation);
                newPopulations.Add(newPop);
            }
            return populations;
        }
    }
}
