using AGIle_Robotics.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperTuple;
using System.Diagnostics;

namespace AGIle_Robotics
{
    public class Population : IPopulation
    {
        public (double, double) WeightRange { get => weightRange; private set => weightRange = value; }
        private (double, double) weightRange;

        public INeuralNetwork[] Networks { get => networks; private set => networks = value; }
        private INeuralNetwork[] networks;

        public Func<double, double> ActivationFunction { get => activationFunction; private set => activationFunction = value; }
        private Func<double, double> activationFunction;

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

        private int size;
        private int[] definition;

        public Population(int size, int[] definition, STuple<double, double> weightRange) => Init(size, definition, weightRange, Math.Tanh);
        public Population(int size, int[] definition, STuple<double, double> weightRange, Func<double, double> activateWith, bool init = true) => Init(size, definition, weightRange, activateWith, init);
        private void Init(int size, int[] definition, STuple<double, double> weightRange, Func<double, double> activateWith, bool init = true)
        {
            WeightRange = weightRange;
            ActivationFunction = activateWith;
            this.size = size;
            this.definition = definition;

            Networks = new INeuralNetwork[size];
            Parallel.For(0, size, index =>
            {
                Networks[index] = new NeuralNetwork(definition, weightRange, activateWith, init);
            });
        }

        async Task<IEvolvable> IEvolvable.Evolve(double transitionRatio, double randomRatio, double mutationRatio) => await Evolve(transitionRatio, randomRatio, mutationRatio);
        public async Task<IPopulation> Evolve(double transitionRatio, double randomRatio, double mutationRatio)
        {
            int len = Networks.Length;
            int transitionAmount = (int)(len * transitionRatio);
            int randomAmount = (int)(len * randomRatio);
            int mutationAmount = (int)(len * mutationRatio);

            List<INeuralNetwork> nextNets = new List<INeuralNetwork>();
            List<INeuralNetwork> remainingNets = await Task.Run(
                () => Networks.OrderByDescending(n => n.Fitness).ToList());

            Task[] tasks = new Task[2];

            tasks[0] = Task.Run(() =>
            nextNets.AddRange(remainingNets.Take(transitionAmount)));

            tasks[1] = Task.Run(() =>
            {
                for (int i = 0; i < randomAmount; i++)
                {
                    int rand = Environment.RandomInt(transitionAmount, remainingNets.Count);
                    nextNets.Add(remainingNets[rand]);
                    remainingNets.RemoveAt(rand);
                }
            });

            await Task.WhenAll(tasks);

            CrossOver(ref nextNets, nextNets.Count, len);

            if(nextNets.Count != len)
            {
                throw new Exception("Could not create enough new networks");
            }

            Population newPopulation = new Population(size, definition, WeightRange, ActivationFunction, false);
            Parallel.For(0, len, index =>
            {
                var net = nextNets[index];
                newPopulation.Networks[index] = net;
                newPopulation.Networks[index].Fitness = 0;

                newPopulation.Networks[index].Mutate(mutationRatio); // Mutate
            });
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
                    int rand = Environment.RandomInt(0, count); // Do not include already crossed over nets
                    nets[j] = nextNets[rand];
                }
                var newNet = nets[0].CrossOver(nets[1], nets[0].Fitness, nets[1].Fitness);
                nextNets.Add((NeuralNetwork)newNet);
            }
        }

        public void ResetBest() => Best = null;
    }
}
