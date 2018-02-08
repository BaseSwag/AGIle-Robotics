using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AGIle_Robotics.Interfaces;
using AGIle_Robotics.Extension;
using SuperTuple;
using AGIle_Robotics.Updater;
using Newtonsoft.Json;
using System.Runtime.Remoting;
using System.Threading;

namespace AGIle_Robotics
{
    public class Trainer : ITrainer
    {
        public enum TrainerActivationType
        {
            Single = 1,
            Pair = 2,
        }

        [JsonIgnore]
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

        [JsonIgnore]
        public Func<INeuralNetwork, Task<double>> SingleFitnessFunction
            { get => singleFitnessFunction; private set => singleFitnessFunction = value; }
        private Func<INeuralNetwork, Task<double>> singleFitnessFunction;

        [JsonIgnore]
        public Func<INeuralNetwork, INeuralNetwork, Task<STuple<double, double>>> PairFitnessFunction
            { get => pairFitnessFunction; private set => pairFitnessFunction = value; }
        private Func<INeuralNetwork, INeuralNetwork, Task<STuple<double, double>>> pairFitnessFunction;

        public int Level
        {
            get => level;
            set
            {
                level = value;
            }
        }
        private int level;

        public INeuralNetwork Best { get => best; private set => best = value; }
        private INeuralNetwork best;

        public TrainerActivationType ActivationType { get => activationType; set => activationType = value; }
        private TrainerActivationType activationType = TrainerActivationType.Single;

        [JsonConstructor]
        public Trainer(double transitionRatio, double randomRatio, double mutationRatio, double creationRatio, IGeneration currentGeneration, int level, INeuralNetwork best, TrainerActivationType activationType)
        {
            TransitionRatio = transitionRatio;
            RandomRatio = randomRatio;
            MutationRatio = mutationRatio;
            CreationRatio = creationRatio;
            CurrentGeneration = currentGeneration;
            Level = level;
            Extensions.StatusUpdater.GenerationLevel = level;
            Best = best;
            ActivationType = activationType;
        }
        public Trainer(double transitionRatio = 0.5, double randomRatio = 0.1, double mutationRatio = 0.1, double creationRatio = 0.1)
        {
            TransitionRatio = transitionRatio;
            RandomRatio = randomRatio;
            MutationRatio = mutationRatio;
            CreationRatio = creationRatio;
        }

        public async Task Initialize(int size, STuple<int, int> popSize, STuple<int, int> ports, STuple<int, int> length, STuple<int, int> width, STuple<double, double> weightRange, Func<double, double> activateWith)
        {
            Extensions.StatusUpdater.Activity = Updater.StatusUpdater.FrameworkActivity.Initializing;
            CurrentGeneration = await Task.Run(
                () => (IGeneration)new Generation(size, popSize, ports, length, width, weightRange, activateWith));
            CurrentGeneration.TransitionRatio = TransitionRatio;
            CurrentGeneration.RandomRatio = RandomRatio;
            CurrentGeneration.MutationRatio = MutationRatio;
            CurrentGeneration.CreationRatio = CreationRatio;
            Extensions.StatusUpdater.Activity = Updater.StatusUpdater.FrameworkActivity.NotReady;
        }

        public async Task Create()
        {
            Extensions.StatusUpdater.Activity = Updater.StatusUpdater.FrameworkActivity.Creating;
            await CurrentGeneration.CreateAsync();
            Level++;
            Extensions.StatusUpdater.Activity = Updater.StatusUpdater.FrameworkActivity.Idle;
        }

        public async Task InitializeAndCreate(int size, STuple<int, int> popSize, STuple<int, int> ports, STuple<int, int> length, STuple<int, int> width, STuple<double, double> weightRange, Func<double, double> activateWith)
        {
            await Initialize(size, popSize, ports, length, width, weightRange, activateWith);
            await Create();
        }

        public async Task Evaluate()
        {
            Extensions.StatusUpdater.Activity = StatusUpdater.FrameworkActivity.Evaluating;
            Extensions.StatusUpdater.EvaluationsLeft = Extensions.GaussSum(Extensions.StatusUpdater.NetworkCount) - Extensions.StatusUpdater.NetworkCount;

            switch (ActivationType)
            {
                case TrainerActivationType.Single:
                    await CurrentGeneration.EvaluateSingle(SingleFitnessFunction);
                    break;
                case TrainerActivationType.Pair:
                    await CurrentGeneration.EvaluatePair(PairFitnessFunction);
                    break;
            }

            Best = CurrentGeneration.Best;
            Extensions.StatusUpdater.Activity = Updater.StatusUpdater.FrameworkActivity.Idle;
        }

        public async Task Evolve()
        {
            Extensions.StatusUpdater.Activity = Updater.StatusUpdater.FrameworkActivity.Evolving;
            Extensions.StatusUpdater.NetworksEvolved = 0;
            CurrentGeneration = (IGeneration) await CurrentGeneration.Evolve(TransitionRatio, RandomRatio, MutationRatio, CreationRatio);
            Interlocked.Increment(ref Extensions.StatusUpdater.generationLevel);
            Interlocked.Increment(ref level);
            Extensions.StatusUpdater.Activity = Updater.StatusUpdater.FrameworkActivity.Idle;
        }

        public async Task EvaluateAndEvolve()
        {
            await Evaluate();
            await Evolve();
        }

        public void SetFitnessFunction(Delegate function)
        {
            // Type unknown = ((ObjectHandle)function).Unwrap().GetType();
            switch (ActivationType)
            {
                case TrainerActivationType.Single:
                    /*
                    if (unknown != typeof(Func<INeuralNetwork, Task<double>>))
                        throw new ArgumentException("FitnessFunction does not match activation type (Single)");
                    */
                    SingleFitnessFunction = (Func<INeuralNetwork, Task<double>>)function;
                    break;
                case TrainerActivationType.Pair:
                    /*
                    if(unknown != typeof(Func<INeuralNetwork, INeuralNetwork, Task<STuple<double, double>>>))
                        throw new ArgumentException("FitnessFunction does not match activation type (Pair)");
                    */
                    PairFitnessFunction = (Func<INeuralNetwork, INeuralNetwork, Task<STuple<double, double>>>)function;
                    break;
            }
        }

        public string Serialize()
        {
            var serializationSettings = new JsonSerializerSettings();
            serializationSettings.TypeNameHandling = TypeNameHandling.All;

            string json = JsonConvert.SerializeObject(this, Formatting.None, serializationSettings);
            return json;
        }

        public static Trainer Deserialize(string json)
        {
            var serializationSettings = new JsonSerializerSettings();
            serializationSettings.TypeNameHandling = TypeNameHandling.All;

            return JsonConvert.DeserializeObject<Trainer>(json, serializationSettings);
        }
    }
}