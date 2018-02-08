using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AGIle_Robotics.Interfaces;
using SuperTuple;
using Newtonsoft.Json;
using AGIle_Robotics.Extension;
using System.Threading;

namespace AGIle_Robotics
{
    public class Generation : IGeneration
    {
        [JsonConverter(typeof(ArrayListJsonConverter<IPopulation>))]
        public IPopulation[] Populations { get => populations; set => populations = value; }
        private IPopulation[] populations;

        public int Size
        {
            get => size;
            private set
            {
                size = value;
                Extensions.StatusUpdater.PopulationCount = value;
            }
        }
        private int size;

        public double TransitionRatio { get => transitionRatio; set => SetRatio(ref transitionRatio, value); }
        private double transitionRatio = 0.5;

        public double RandomRatio { get => randomRatio; set => SetRatio(ref randomRatio, value); }
        private double randomRatio = 0.1;

        public double MutationRatio { get => mutationRatio; set => SetRatio(ref mutationRatio, value); }
        private double mutationRatio = 0.1;

        public double CreationRatio { get => creationRatio; set => SetRatio(ref creationRatio, value); }
        private double creationRatio = 0.1;

        private void SetRatio(ref double ratio, double value)
        {
            double temp = ratio;
            ratio = value;

            if(TransitionRatio + RandomRatio + MutationRatio + CreationRatio >= 1)
            {
                ratio = temp;
                throw new ArgumentException("Ratio values exceed 1.0");
            }
        }

        [JsonProperty]
        public INeuralNetwork Best
        {
            get
            {
                if (best != null) return best;

                double highest = double.MinValue;
                int pop = 0;
                for(int p = 0; p < Populations.Length; p++)
                {
                    var f = Populations[p].Best.Fitness;
                    Extensions.StatusUpdater.PopulationFitnesses[p] = f;
                    if(f > highest)
                    {
                        highest = f;
                        pop = p;
                    }
                }
                best = Populations[pop].Best;
                Extensions.StatusUpdater.BestFitness = best.Fitness;
                Extensions.StatusUpdater.FitnessHistory.Add(best.Fitness);
                return best;
            }
            private set => best = value;
        }
        private INeuralNetwork best;

        [JsonConverter(typeof(IntTupleJsonConverter))]
        public (int, int) Ports { get => ports; private set => ports = value; }
        private (int, int) ports;

        [JsonConverter(typeof(IntTupleJsonConverter))]
        public (int, int) PopulationSize { get => populationSize; private set => populationSize = value; }
        private (int, int) populationSize;

        [JsonConverter(typeof(DoubleTupleJsonConverter))]
        public (double, double) WeightRange { get => weightRange; private set => weightRange = value; }
        private (double, double) weightRange;

        [JsonIgnore]
        public Func<double, double> ActivationFunction { get => activationFunction; private set => activationFunction = value; }
        private Func<double, double> activationFunction = Math.Tanh;

        [JsonConverter(typeof(IntTupleJsonConverter))]
        public (int, int) Length { get => length; private set => length = value; }
        private (int, int) length;

        [JsonConverter(typeof(IntTupleJsonConverter))]
        public (int, int) Width { get => width; private set => width = value; }
        private (int, int) width;

        [JsonConstructor]
        public Generation(IPopulation[] populations, int size, STuple<int, int> populationSize, STuple<int, int> ports, STuple<int, int> length, STuple<int, int> width, STuple<double, double> weightRange, double transitionRatio, double randomRatio, double mutationRatio, double creationRatio)
        {
            Size = size;
            Populations = populations;
            int networkCount = 0;
            for(int i = 0; i < Populations.Length; i++)
            {
                networkCount += Populations[i].Size;
            }
            Extensions.StatusUpdater.NetworkCount = networkCount;
            PopulationSize = populationSize;
            Ports = ports;

            Length = length;
            Width = width;

            WeightRange = weightRange;

            ActivationFunction = Math.Tanh;

            TransitionRatio = transitionRatio;
            RandomRatio = randomRatio;
            MutationRatio = mutationRatio;
            CreationRatio = creationRatio;
        }
        public Generation(int size, STuple<int, int> popSize, STuple<int, int> ports, STuple<int, int> length, STuple<int, int> width, STuple<double, double> weightRange, Func<double, double> activateWith)
        {
            Size = size;
            PopulationSize = popSize;
            Ports = ports;

            Length = length;
            Width = width;

            WeightRange = weightRange;

            ActivationFunction = activateWith;
        }

        public void Create() => CreateAsync().Wait();
        public async Task CreateAsync()
        {
            Task<IPopulation>[] tasks = Extensions.WorkPool.For(0, Size, () =>
            {
                var population = MakePopulation();
                population.Create();
                return population;
            });
            Populations = await Task.WhenAll(tasks);
            int networkCount = 0;
            for(int i = 0; i < Populations.Length; i++)
            {
                networkCount += Populations[i].Size;
            }
            Extensions.StatusUpdater.NetworkCount = networkCount;
        }

        private IPopulation MakePopulation()
        {
            var size = Extensions.RandomInt(PopulationSize);
            var length = Extensions.RandomInt(Length);
            var definition = new int[length];

            for(int i = 0; i < definition.Length - 1; i++)
            {
                var width = Extensions.RandomInt(Width);
                definition[i] = width;
            }
            definition[length - 1] = Ports.Item2;

            var newPop = new Population(size, definition, WeightRange, Ports.Item1, ActivationFunction);
            return newPop;
        }

        public async Task EvaluateSingle(Func<INeuralNetwork, Task<double>> fitnessFunction)
        {
            List<Task> tasks = new List<Task>();
            await ResetFitness();
            for (int pop = 0; pop < Populations.Length; pop++)
            {
                int pop2 = pop;
                for (int net = 0; net < Populations[pop2].Networks.Length; net++)
                {
                    int net2 = net;
                    var t = Extensions.WorkPool.Enqueue(async () =>
                    {
                        Interlocked.Increment(ref Extensions.StatusUpdater.evaluationsRunning);
                        Populations[pop2].Networks[net2].Fitness = await fitnessFunction(Populations[pop2].Networks[net2]);
                        Interlocked.Decrement(ref Extensions.StatusUpdater.evaluationsRunning);
                        Interlocked.Decrement(ref Extensions.StatusUpdater.evaluationsLeft);
                    });
                    tasks.Add(t);
                }
            }
            await Task.WhenAll(tasks);
            Best = null;
        }

        public async Task EvaluatePair(Func<INeuralNetwork, INeuralNetwork, Task<STuple<double, double>>> fitnessFunction)
        {
            await ResetFitness();

            List<Task> tasks = new List<Task>();

            EvaluatePair(fitnessFunction, ref tasks, 0, 0);

            await Task.WhenAll(tasks);

            Best = null;
        }
        private void EvaluatePair(Func<INeuralNetwork, INeuralNetwork, Task<STuple<double, double>>> fitnessFunction, ref List<Task> tasks, int pop, int net)
        {
            var p = pop;
            var n = net;
            var t = new Task<object>(() => PairEvaluationCycle(fitnessFunction, p, n).Result);
            tasks.Add(t);
            Extensions.WorkPool.Enqueue(t);

            int nextPop, nextNet;

            if(net < Populations[pop].Networks.Length - 1)
            {
                nextPop = pop;
                nextNet = net + 1;
            }
            else
            {
                nextPop = pop + 1;
                if (nextPop >= Populations.Length) return;
                nextNet = 0;
            }

            EvaluatePair(fitnessFunction, ref tasks, nextPop, nextNet);
        }
        private Task<object[]> PairEvaluationCycle(Func<INeuralNetwork, INeuralNetwork, Task<STuple<double, double>>> fitnessFunction, int pop, int net)
        {
            var myNet = Populations[pop].Networks[net];
            var tasks = new List<Task<object>>();
            for (int p = pop; p < Populations.Length; p++)
            {
                int p2 = p;
                for (int n = net + 1; n < Populations[p2].Networks.Length; n++)
                {
                    int n2 = n;
                    var enemyNet = Populations[p2].Networks[n2];
                    var tcs = new TaskCompletionSource<object>();
                    var t = PairEvaluationCycle(fitnessFunction, (pop, net), myNet, (p2, n2), enemyNet);
                    t.ContinueWith(finished =>
                    {
                        lock(Populations[pop].Networks[net])
                            Populations[pop].Networks[net].Fitness += finished.Result.Item1;
                        lock(Populations[p2].Networks[n2])
                            Populations[p2].Networks[n2].Fitness += finished.Result.Item2;
                        Interlocked.Decrement(ref Extensions.StatusUpdater.evaluationsRunning);
                        Interlocked.Decrement(ref Extensions.StatusUpdater.evaluationsLeft);
                        tcs.SetResult(null);
                    });
                    tasks.Add(tcs.Task);
                }
            }
            return Task.WhenAll(tasks);
        }
        public Task<STuple<double, double>> PairEvaluationCycle(Func<INeuralNetwork, INeuralNetwork, Task<STuple<double, double>>> fitnessFunction, (int p, int n) i1, INeuralNetwork n1, (int p, int n) i2, INeuralNetwork n2)
        {
            Interlocked.Increment(ref Extensions.StatusUpdater.evaluationsRunning);
            return fitnessFunction(n1, n2);
        }

        private async Task ResetFitness()
        {
            await Extensions.WorkPool.ForToTask(0, Populations.Length, p =>
            {
                Populations[p].ResetBest();
                for(int n = 0; n < Populations[p].Networks.Length; n++)
                    Populations[p].Networks[n].Fitness = 0;
            });
        }

        public Task<IGeneration> Evolve() => Evolve(TransitionRatio, RandomRatio, MutationRatio, CreationRatio);
        public async Task<IGeneration> Evolve(double transitionRatio, double randomRatio, double mutationRatio, double creationRatio)
        {
            Task<IPopulation>[] tasks = Extensions.WorkPool.For(0, Size, i
                => Populations[i].Evolve(transitionRatio, randomRatio, mutationRatio, creationRatio).Result);
            
            Generation newGen = new Generation(Size, PopulationSize, Ports, Length, Width, WeightRange, activationFunction);
            newGen.Populations = await Task.WhenAll(tasks);

            return newGen;
        }
        async Task<IEvolvable> IEvolvable.Evolve(double transitionRatio, double randomRatio, double mutationRatio, double creationRatio)
        {
            return (IEvolvable) await Evolve(transitionRatio, randomRatio, mutationRatio, creationRatio);
        }
    }
}
