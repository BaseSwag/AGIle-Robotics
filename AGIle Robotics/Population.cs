using AGIle_Robotics.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public INeuralNetwork Best { get => best; private set => best = value; }
        private INeuralNetwork best;

        private int size;
        private int[] definition;

        public Population(int size, int[] definition, (double, double) weightRange) => Init(size, definition, weightRange, Math.Tanh);
        public Population(int size, int[] definition, (double, double) weightRange, Func<double, double> activateWith, bool init = true) => Init(size, definition, weightRange, activateWith, init);
        private void Init(int size, int[] definition, (double, double) weightRange, Func<double, double> activateWith, bool init = true)
        {
            WeightRange = weightRange;
            ActivationFunction = activateWith;
            this.size = size;
            this.definition = definition;

            Networks = new INeuralNetwork[size];
            for(int i = 0; i < size; i++)
            {
                Networks[i] = new NeuralNetwork(definition, weightRange, activateWith, init);
            }
        }

        public IEvolvable Evolve(double transitionRatio, double randomRatio, double mutationRatio)
        {
            int len = Networks.Length;
            int transitionAmount = (int)(len * transitionRatio);
            int randomAmount = (int)(len * randomRatio);
            int mutationAmount = (int)(len * mutationRatio);

            // TODO: Stop killing performance

            List<INeuralNetwork> nextNets = new List<INeuralNetwork>();
            List<INeuralNetwork> remainingNets = Networks.OrderByDescending(n => n.Fitness).ToList();
            int count = 0;
            int left = len;

            ChooseBest(ref nextNets, ref remainingNets, transitionAmount);
            count += transitionAmount;
            left -= transitionAmount;

            ChooseRandom(ref nextNets, ref remainingNets, ref left, randomAmount);
            count += randomAmount;

            CrossOver(ref nextNets, count, len);

            if(nextNets.Count != len)
            {
                throw new Exception("Could not create enough new networks");
            }

            Population newPopulation = new Population(size, definition, WeightRange, ActivationFunction, false);
            for(int i = 0; i < len; i++)
            {
                var net = nextNets[i];
                newPopulation.Networks[i] = net;
                newPopulation.Networks[i].Fitness = 0;

                if(Best == null || Best.Fitness < net.Fitness)
                {
                    Best = net;
                }

                newPopulation.Networks[i].Mutate(mutationRatio); // Mutate
            }
            return newPopulation;
        }

        private void ChooseBest(ref List<INeuralNetwork> nextNets, ref List<INeuralNetwork> remainingNets, int amount)
        {
            for(int i = 0; i < amount; i++)
            {
                nextNets.Add(remainingNets[0]);
                remainingNets.RemoveAt(0);
            }
        }

        private void ChooseRandom(ref List<INeuralNetwork> nextNets, ref List<INeuralNetwork> remainingNets, ref int left, int amount)
        {
            for(int i = 0; i < amount; i++)
            {
                int rand = Environment.RandomInt(0, left);
                nextNets.Add(remainingNets[rand]);
                remainingNets.RemoveAt(rand);
                left--;
            }
        }

        private void CrossOver(ref List<INeuralNetwork> nextNets, int count, int total)
        {
            int left = total - count;
            for(int i = 0; i < left; i++)
            {
                int rand = Environment.RandomInt(0, count); // Do not include already crossed over nets
                var net1 = nextNets[rand];
                rand = Environment.RandomInt(0, count); // Do not include already crossed over nets
                var net2 = nextNets[rand];
                var newNet = net1.CrossOver(net2, net1.Fitness, net2.Fitness);
                nextNets.Add((NeuralNetwork)newNet);
            }
        }
    }
}
