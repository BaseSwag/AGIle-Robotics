using AGIle_Robotics.Interfaces;
using AGIle_Robotics.Extension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperTuple;
using System.Diagnostics;
using Newtonsoft.Json;
using System.Threading;

namespace AGIle_Robotics
{
    public class Population : IPopulation
    {
        [JsonConverter(typeof(DoubleTupleJsonConverter))]
        public (double, double) WeightRange { get => weightRange; private set => weightRange = value; }
        private (double, double) weightRange;

        [JsonConverter(typeof(ArrayListJsonConverter<INeuralNetwork>))]
        public INeuralNetwork[] Networks { get => networks; private set => networks = value; }
        private INeuralNetwork[] networks;

        [JsonIgnore]
        public Func<double, double> ActivationFunction { get => activationFunction; private set => activationFunction = value; }
        private Func<double, double> activationFunction = Math.Tanh;

        [JsonProperty]
        public INeuralNetwork Best
        {
            get
            {
                if (best != null) return best;

                double highest = double.MinValue;
                int net = 0;
                for(int n = 0; n < Networks.Length; n++)
                {
                    if(Networks[n].Fitness > highest)
                    {
                        highest = Networks[n].Fitness;
                        net = n;
                    }
                }
                best = Networks[net];
                return best;
            }
            private set => best = value;
        }
        public INeuralNetwork best;

        public int Size { get => size; private set => size = value; }
        private int size;

        public int[] Definition { get => definition; private set => definition = value; }
        private int[] definition;

        public int InputSize { get => inputSize; private set => inputSize = value; }
        private int inputSize;

        //public Population(int size, int[] definition, STuple<double, double> weightRange) => Init(size, definition, weightRange, Math.Tanh);
        [JsonConstructor]
        public Population(INeuralNetwork[] networks, int size, int[] definition, STuple<double, double> weightRange, int inputSize)
        {
            WeightRange = weightRange;
            ActivationFunction = Math.Tanh;
            Size = size;
            Definition = definition;
            InputSize = inputSize;

            Networks = networks;
        }
        public Population(int size, int[] definition, STuple<double, double> weightRange, int inputSize, Func<double, double> activateWith)
        {
            WeightRange = weightRange;
            ActivationFunction = activateWith;
            Size = size;
            Definition = definition;
            InputSize = inputSize;

            Networks = new INeuralNetwork[size];
        }

        public void Create()
        {
            for(int i = 0; i < Size; i++)
            {
                var network = new NeuralNetwork(Definition, WeightRange, InputSize, ActivationFunction);
                Networks[i] = network;
                Networks[i].Create();
            }
        }

        async Task<IEvolvable> IEvolvable.Evolve(double transitionRatio, double randomRatio, double mutationRatio, double creationRatio) => await Evolve(transitionRatio, randomRatio, mutationRatio, creationRatio);
        public async Task<IPopulation> Evolve(double transitionRatio, double randomRatio, double mutationRatio, double creationRatio)
        {
            int len = Networks.Length;
            int transitionAmount = (int)(len * transitionRatio) - 1;
            int randomAmount = (int)(len * randomRatio);
            int mutationAmount = (int)(len * mutationRatio);
            int creationAmount = (int)(len * creationRatio);

            List<INeuralNetwork> nextNets = new List<INeuralNetwork>();
            List<INeuralNetwork> remainingNets = Networks.OrderByDescending(n => n.Fitness).ToList();

            // Take fist twice to prevent mutation on best once
            nextNets.Add(remainingNets[0]);
            nextNets.AddRange(remainingNets.Take(transitionAmount));

            for (int i = 0; i < randomAmount; i++)
            {
                int rand = Extensions.RandomInt(transitionAmount, remainingNets.Count);
                nextNets.Add(remainingNets[rand]);
                remainingNets.RemoveAt(rand);
            }

            for (int i = 0; i < creationAmount; i++)
            {
                var newNet = new NeuralNetwork(definition, WeightRange, InputSize, ActivationFunction);
                await Task.Run(() => newNet.Create());
                nextNets.Add(newNet);
            }

            CrossOver(ref nextNets, nextNets.Count, len);

            if(nextNets.Count != len)
            {
                throw new Exception("Could not create enough or too many new networks");
            }

            Population newPopulation = new Population(size, definition, WeightRange, InputSize, ActivationFunction);
            for(int i = 0; i < len; i++)
            {
                var net = nextNets[i];
                newPopulation.Networks[i] = net;
                newPopulation.Networks[i].Fitness = 0;

                if(i > 0) // Do not mutate best
                    newPopulation.Networks[i].Mutate(mutationRatio); // Mutate
                Interlocked.Increment(ref Extensions.StatusUpdater.networksEvolved);
            }

            /*
            TaskCompletionSource<IPopulation> tcs = new TaskCompletionSource<IPopulation>();
            tcs.SetResult(newPopulation);
            */
            return newPopulation;
        }

        private void CrossOver(ref List<INeuralNetwork> nextNets, int count, int total)
        {
            int left = total - count;
            for(int i = 0; i < left; i++)
            {
                var nets = new INeuralNetwork[2];
                for(int j = 0; j < 2; j++)
                {
                    int rand = Extensions.RandomInt(0, count); // Do not include already crossed over nets
                    nets[j] = nextNets[rand];
                }
                var newNet = nets[0].CrossOver(nets[1], nets[0].Fitness, nets[1].Fitness);
                nextNets.Add((NeuralNetwork)newNet);
            }
        }

        public void ResetBest() => Best = null;
    }
}
