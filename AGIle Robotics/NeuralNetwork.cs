using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AGIle_Robotics.Interfaces;
using AGIle_Robotics.Extension;
using SuperTuple;

namespace AGIle_Robotics
{
    public class NeuralNetwork : INeuralNetwork
    {
        public ILayer[] Layers { get => layers; private set => layers = value; }
        private ILayer[] layers;

        public int InputSize => Layers.Length > 0 ? Layers[0].Neurons.Length : 0;

        public int OutputSize => Layers.Length > 0 ? Layers[Layers.Length - 1].Neurons.Length : 0;

        public (double, double) WeightRange { get => weightRange; private set => weightRange = value; }
        private (double, double) weightRange;

        public double Fitness { get => fitness; set => fitness = value; }
        private double fitness;

        public Func<double, double> ActivationFunction { get => activationFunction; set => activationFunction = value; }
        private Func<double, double> activationFunction;

        public int[] Definition { get => definition; set => definition = value; }
        private int[] definition;

        public NeuralNetwork(int[] definition, STuple<double, double> weightRange, Func<double, double> activateWith, bool init = true)
        {
            WeightRange = weightRange;
            Definition = definition;
            ActivationFunction = activateWith;

            Layers = new ILayer[definition.Length];
            Extensions.TaskFor(0, definition.Length, index =>
            {
                int inputSize = index > 0 ? definition[index - 1] : definition[index];
                ILayer layer = new Layer(definition[index], inputSize, weightRange, activateWith, init);
                Layers[index] = layer;
            });
        }

        public double[] Activate(double[] input)
        {
            for (int i = 0; i < Layers.Length; i++)
            {
                input = Layers[i].Activate(input);
            }
            return input;
        }

        public async Task<double[]> ActivateAsync(double[] input)
        {
            for (int i = 0; i < Layers.Length; i++)
            {
                input = await Layers[i].ActivateAsync(input);
            }
            return input;
        }

        public INeuralElement CrossOver(INeuralElement e, double p1, double p2)
        {
            var task = CrossOverAsync(e, p1, p2);
            task.Wait();
            return task.Result;
        }
        public async Task<INeuralElement> CrossOverAsync(INeuralElement e, double p1, double p2)
        {
            var net2 = e as NeuralNetwork;
            int len = Layers.Length;

            if(len == net2?.Layers.Length && InputSize == net2.InputSize && OutputSize == net2.OutputSize)
            {
                var newNetwork = new NeuralNetwork(Definition, WeightRange, ActivationFunction, false);
                await Extensions.TaskForAsync(0, len, index =>
                {
                    newNetwork.Layers[index] = (ILayer)Layers[index].CrossOver(net2.Layers[index], p1, p2);
                });
                return newNetwork;
            }
            else
            {
                throw new InvalidOperationException("Cannot perform CrossOver on Networks with different definitions.");
            }
        }

        public void Mutate(double ratio) => MutateAsync(ratio).Wait();
        public Task MutateAsync(double ratio)
        {
            return Extensions.TaskForAsync(0, Layers.Length, index =>
                Layers[index].Mutate(ratio));
        }
    }
}
