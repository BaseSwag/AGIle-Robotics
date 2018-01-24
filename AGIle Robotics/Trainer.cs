using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AGIle_Robotics.Interfaces;
using SuperTuple;

namespace AGIle_Robotics
{
    public class Trainer : ITrainer
    {
        public double TransitionRatio { get => transitionRatio; set => transitionRatio = value; }
        private double transitionRatio;
        public double RandomRatio { get => randomRatio; set => randomRatio = value; }
        private double randomRatio;
        public double MutationRatio { get => mutationRatio; set => mutationRatio = value; }
        private double mutationRatio;

        public IGeneration CurrentGeneration { get => currentGeneration; private set => currentGeneration = value; }
        private IGeneration currentGeneration;

        public Func<INeuralNetwork, INeuralNetwork, Task<STuple<double, double>>> FitnessFunction
            { get => fitnessFunction; set => fitnessFunction = value; }
        private Func<INeuralNetwork, INeuralNetwork, Task<STuple<double, double>>> fitnessFunction;

        public int Level { get => level; private set => level = value; }
        private int level;

        public Trainer(double transitionRatio = 0.5, double randomRatio = 0.1, double mutationRatio = 0.1)
        {
            TransitionRatio = transitionRatio;
            RandomRatio = randomRatio;
            MutationRatio = mutationRatio;
        }

        public async Task Initialize(int size, STuple<int, int> popSize, STuple<int, int> ports, STuple<int, int> length, STuple<int, int> width, STuple<double, double> weightRange, Func<double, double> activateWith)
        {
            CurrentGeneration = await Task.Run(
                () => (IGeneration)new Generation(size, popSize, ports, length, width, weightRange, activateWith));
        }

        public async Task Create()
        {
            await CurrentGeneration.Create();
            Level++;
        }

        public async Task InitializeAndCreate(int size, STuple<int, int> popSize, STuple<int, int> ports, STuple<int, int> length, STuple<int, int> width, STuple<double, double> weightRange, Func<double, double> activateWith)
        {
            await Initialize(size, popSize, ports, length, width, weightRange, activateWith);
            await Create();
        }

        public Task Evaluate() => CurrentGeneration.Evaluate(fitnessFunction);

        public Task Evaluate(Func<INeuralNetwork, INeuralNetwork, Task<STuple<double, double>>> fitnessFunction)
            => CurrentGeneration.Evaluate(fitnessFunction);

        public async Task Evolve()
        {
            CurrentGeneration = (IGeneration) await CurrentGeneration.Evolve(TransitionRatio, RandomRatio, MutationRatio);
            Level++;
        }

        public async Task EvaluateAndEvolve()
        {
            await Evaluate();
            await Evolve();
        }

        public async Task EvaluateAndEvolve(Func<INeuralNetwork, INeuralNetwork, Task<STuple<double, double>>> fitnessFunction)
        {
            await Evaluate(fitnessFunction);
            await Evolve();
        }
    }
}
