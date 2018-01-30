using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AGIle_Robotics.Interfaces;
using AGIle_Robotics.Extension;
using SuperTuple;
using AGIle_Robotics.Updater;

namespace AGIle_Robotics
{
    public class Trainer : ITrainer
    {
        public StatusUpdater StatusUpdater => Extensions.StatusUpdater;

        public double TransitionRatio
        {
            get => transitionRatio;
            set
            {
                transitionRatio = value;
                Extensions.StatusUpdater.TransitionRatio = value;
            }
        }
        private double transitionRatio;
        public double RandomRatio
        {
            get => randomRatio;
            set
            {
                randomRatio = value;
                Extensions.StatusUpdater.RandomRatio = value;
            }
        }
        private double randomRatio;
        public double MutationRatio
        {
            get => mutationRatio;
            set
            {
                mutationRatio = value;
                Extensions.StatusUpdater.MutationRatio = value;
            }
        }
        private double mutationRatio;
        public double CreationRatio
        {
            get => creationRatio;
            set
            {
                creationRatio = value;
                Extensions.StatusUpdater.CreationRatio = value;
            }
        }
        private double creationRatio;

        public IGeneration CurrentGeneration { get => currentGeneration; private set => currentGeneration = value; }
        private IGeneration currentGeneration;

        public Func<INeuralNetwork, INeuralNetwork, Task<STuple<double, double>>> FitnessFunction
            { get => fitnessFunction; set => fitnessFunction = value; }
        private Func<INeuralNetwork, INeuralNetwork, Task<STuple<double, double>>> fitnessFunction;

        public int Level
        {
            get => level;
            set
            {
                level = value;
                Extensions.StatusUpdater.GenerationLevel = value;
            }
        }
        private int level;

        public INeuralNetwork Best { get => best; private set => best = value; }
        private INeuralNetwork best;

        public Trainer(double transitionRatio = 0.5, double randomRatio = 0.1, double mutationRatio = 0.1)
        {
            TransitionRatio = transitionRatio;
            RandomRatio = randomRatio;
            MutationRatio = mutationRatio;
        }

        public async Task Initialize(int size, STuple<int, int> popSize, STuple<int, int> ports, STuple<int, int> length, STuple<int, int> width, STuple<double, double> weightRange, Func<double, double> activateWith)
        {
            Extensions.StatusUpdater.Activity = Updater.StatusUpdater.FrameworkActivity.Initializing;
            CurrentGeneration = await Task.Run(
                () => (IGeneration)new Generation(size, popSize, ports, length, width, weightRange, activateWith));
            Extensions.StatusUpdater.Activity = Updater.StatusUpdater.FrameworkActivity.NotReady;
        }

        public async Task Create()
        {
            Extensions.StatusUpdater.Activity = Updater.StatusUpdater.FrameworkActivity.Creating;
            await CurrentGeneration.Create();
            Level++;
            Extensions.StatusUpdater.Activity = Updater.StatusUpdater.FrameworkActivity.Idle;
        }

        public async Task InitializeAndCreate(int size, STuple<int, int> popSize, STuple<int, int> ports, STuple<int, int> length, STuple<int, int> width, STuple<double, double> weightRange, Func<double, double> activateWith)
        {
            await Initialize(size, popSize, ports, length, width, weightRange, activateWith);
            await Create();
        }

        public Task Evaluate() => Evaluate(fitnessFunction);

        public async Task Evaluate(Func<INeuralNetwork, INeuralNetwork, Task<STuple<double, double>>> fitnessFunction)
        {
            Extensions.StatusUpdater.Activity = Updater.StatusUpdater.FrameworkActivity.Evaluating;
            await CurrentGeneration.Evaluate(fitnessFunction);
            Best = CurrentGeneration.Best;
            Extensions.StatusUpdater.Activity = Updater.StatusUpdater.FrameworkActivity.Idle;
        }

        public async Task Evolve()
        {
            Extensions.StatusUpdater.Activity = Updater.StatusUpdater.FrameworkActivity.Evolving;
            CurrentGeneration = (IGeneration) await CurrentGeneration.Evolve(TransitionRatio, RandomRatio, MutationRatio);
            Level++;
            Extensions.StatusUpdater.Activity = Updater.StatusUpdater.FrameworkActivity.Idle;
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
