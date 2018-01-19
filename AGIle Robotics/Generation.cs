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

        public int Size { get => size; private set => size = value; }
        private int size;

        public double TransitionRatio { get => transitionRatio; set => transitionRatio = value; }
        private double transitionRatio = 0.5;

        public double RandomRatio { get => randomRatio; set => randomRatio = value; }
        private double randomRatio = 0.1;

        public double MutationRatio { get => mutationRatio; set => mutationRatio = value; }
        private double mutationRatio = 0.1;

        public INeuralNetwork Best
        {
            get
            {
                if (best != null) return best;

                double highest = double.MinValue;
                int pop = 0;
                for(int p = 0; p < Populations.Length; p++)
                {
                    if(Populations[p].Best.Fitness > highest)
                    {
                        highest = Populations[p].Best.Fitness;
                        pop = p;
                    }
                }
                best = Populations[pop].Best;
                return best;
            }
            private set => best = value;
        }
        public INeuralNetwork best;

        public (int, int) Ports { get => ports; private set => ports = value; }
        public (int, int) ports;

        public (int, int) PopulationSize { get => populationSize; private set => populationSize = value; }
        public (int, int) populationSize;

        public (double, double) WeightRange { get => weightRange; private set => weightRange = value; }
        private (double, double) weightRange;

        public Func<double, double> ActivationFunction { get => activationFunction; private set => activationFunction = value; }
        private Func<double, double> activationFunction = Math.Tanh;

        public (int, int) Length { get => length; private set => length = value; }
        public (int, int) length;

        public (int, int) Width { get => width; private set => width = value; }
        public (int, int) width;

        public Generation(int size, (int, int) popSize, (int, int) ports, (int, int) length, (int, int) width, (double, double) weightRange)
            => Init(size, popSize, ports, length, width, weightRange, Math.Tanh);
        public Generation(int size, (int, int) popSize, (int, int) ports, (int, int) length, (int, int) width, (double, double) weightRange, Func<double, double> activateWith)
            => Init(size, popSize, ports, length, width, weightRange, activateWith);
        private void Init(int size, (int, int) popSize, (int, int) ports, (int, int) length, (int, int) width, (double, double) weightRange, Func<double, double> activateWith)
        {
            Size = size;
            PopulationSize = popSize;
            Ports = ports;

            Length = length;
            Width = width;

            WeightRange = weightRange;

            ActivationFunction = activateWith;
        }

        public async Task Create()
        {
            Task<IPopulation>[] tasks = new Task<IPopulation>[Size];

            for(int i = 0; i < Size; i++)
            {
                var t = new Task<IPopulation>(() => CreatePopulation());
                tasks[i] = t;

                Environment.WorkPool.EnqueueTask(t);
            }

            Populations = await Task.WhenAll(tasks);
        }
        private IPopulation CreatePopulation()
        {
            var size = Environment.RandomInt(PopulationSize);
            var length = Environment.RandomInt(Length);
            var definition = new int[length];

            definition[0] = Ports.Item1;
            definition[length - 1] = Ports.Item2;
            for(int i = 1; i < definition.Length - 1; i++)
            {
                var width = Environment.RandomInt(Width);
                definition[i] = width;
            }

            var newPop = new Population(size, definition, WeightRange, ActivationFunction);
            return newPop;
        }

        public async void Evaluate(Func<INeuralNetwork, INeuralNetwork, Task<Tuple<double, double>>> fitnessFunction)
        {
            await ResetFitness();

            List<Task> tasks = new List<Task>();

            Evaluate(fitnessFunction, ref tasks, 0, 0);

            await Task.WhenAll(tasks.ToArray());

            Best = null;
        }
        private void Evaluate(Func<INeuralNetwork, INeuralNetwork, Task<Tuple<double, double>>> fitnessFunction, ref List<Task> tasks, int pop, int net)
        {
            var p = pop;
            var n = net;
            var t = new Task(async () => await EvaluationCycle(fitnessFunction, p, n));
            tasks.Add(t);
            Environment.WorkPool.EnqueueTask(t);

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

            Evaluate(fitnessFunction, ref tasks, nextPop, nextNet);
        }
        private async Task EvaluationCycle(Func<INeuralNetwork, INeuralNetwork, Task<Tuple<double, double>>> fitnessFunction, int pop, int net)
        {
            var myNet = Populations[pop].Networks[net];
            for(int p = pop; p < Populations.Length; p++)
            {
                for(int n = net; n < Populations[pop].Networks.Length; n++)
                {
                    var enemyNet = Populations[p].Networks[n];
                    var result = await fitnessFunction(myNet, enemyNet);
                    
                    Populations[pop].Networks[net].Fitness += result.Item1;
                    Populations[p].Networks[n].Fitness += result.Item2;
                }
            }
        }

        private async Task ResetFitness()
        {
            await Task.Run(() =>
            {
                for (int p = 0; p < Populations.Length; p++)
                {
                    Populations[p].ResetBest();
                    for (int n = 0; n < Populations[p].Networks.Length; n++)
                        Populations[p].Networks[n].Fitness = 0;
                }
            });
        }

        public async Task<IGeneration> Evolve()
        {
            return (IGeneration) await Evolve(TransitionRatio, RandomRatio, MutationRatio);
        }
        public async Task<IEvolvable> Evolve(double transitionRatio, double randomRatio, double mutationRatio)
        {
            Task<IPopulation>[] tasks = new Task<IPopulation>[Size];

            for(int i = 0; i < Size; i++)
            {
                var x = i;
                var t = new Task<IPopulation>(
                    () => (IPopulation)Populations[x].Evolve(TransitionRatio, RandomRatio, MutationRatio));
                tasks[i] = t;

                Environment.WorkPool.EnqueueTask(t);
            }

            Generation newGen = new Generation(Size, PopulationSize, Ports, Length, Width, WeightRange, activationFunction);
            newGen.Populations = await Task.WhenAll(tasks);

            return newGen;
        }
    }
}
