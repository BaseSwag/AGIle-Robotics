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
    public class Layer : ILayer
    {
        public INeuron[] Neurons { get => neurons; private set => neurons = value; }
        private INeuron[] neurons;

        public (double, double) WeightRange { get => weightRange; private set => weightRange = value; }
        private (double, double) weightRange;

        public Func<double, double> ActivationFunction { get => activationFunction; private set => activationFunction = value; }
        private Func<double, double> activationFunction;

        private int inputSize;

        public Layer(int size, int inputSize, STuple<double, double> weightRange, Func<double, double> activateWith, bool init = true)
        {
            WeightRange = weightRange;
            ActivationFunction = activateWith;
            this.inputSize = inputSize;

            Neurons = new INeuron[size];
            for(int i = 0; i < Neurons.Length; i++)
            {
                Neurons[i] = new Neuron(inputSize, weightRange, activateWith, init);
            }
        }
        public Layer(Neuron[] neurons)
        {
            Neurons = neurons;
        }

        public double[] Activate(double[] input)
        {
            var output = new double[Neurons.Length];
            for(int i = 0; i < Neurons.Length; i++)
            {
                output[i] = Neurons[i].Activate(input)[0];
            }
            return output;
        }

        public Task<double[]> ActivateAsync(double[] input)
        {
            return Task.WhenAll(Extensions.WorkPool.For(0, Neurons.Length, i => Neurons[i].ActivateAsync(input).Result[0]));
            /*
                WorkPool workPool = new WorkPool(Environment.WorkCapacity);
                Task<double>[] tasks = new Task<double>[Neurons.Length];

                for (int i = 0; i < Neurons.Length; i++)
                {
                    var x = i;

                    Task<double> t = new Task<double>(() => Neurons[x].ActivateAsync(input).Result[0]);
                    tasks[i] = t;
                    workPool.EnqueueTask(t);
                }
            */
        }

        public INeuralElement CrossOver(INeuralElement e, double p1, double p2)
        {
            var layer2 = e as Layer;
            int len = Neurons.Length;

            if(len == layer2?.Neurons.Length && inputSize == layer2.inputSize)
            {
                var newLayer = new Layer(len, inputSize, WeightRange, ActivationFunction, false);
                for(int i = 0; i < len; i++)
                {
                    newLayer.Neurons[i] = (INeuron)Neurons[i].CrossOver(layer2.Neurons[i], p1, p2);
                }
                return newLayer;
            }
            else
            {
                throw new InvalidOperationException("Cannot perform CrossOver on Layers with different inputs.");
            }
        }

        public void Mutate(double ratio)
        {
            for(int i = 0; i < Neurons.Length; i++)
                Neurons[i].Mutate(ratio);
        }
    }
}
