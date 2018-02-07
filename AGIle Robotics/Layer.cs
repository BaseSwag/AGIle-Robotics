using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AGIle_Robotics.Interfaces;
using AGIle_Robotics.Extension;
using SuperTuple;
using Newtonsoft.Json;

namespace AGIle_Robotics
{
    public class Layer : ILayer
    {
        [JsonConverter(typeof(ArrayListJsonConverter<INeuron>))]
        public INeuron[] Neurons { get => neurons; private set => neurons = value; }
        private INeuron[] neurons;

        [JsonConverter(typeof(DoubleTupleJsonConverter))]
        public (double, double) WeightRange { get => weightRange; private set => weightRange = value; }
        private (double, double) weightRange;

        [JsonIgnore]
        public Func<double, double> ActivationFunction { get => activationFunction; private set => activationFunction = value; }
        private Func<double, double> activationFunction = Math.Tanh;

        public int Size { get => size; private set => size = value; }
        private int size;

        public int InputSize { get => inputSize; private set => inputSize = value; }
        private int inputSize;

        [JsonConstructor]
        public Layer(INeuron[] neurons, int size, int inputSize, STuple<double, double> weightRange)
        {
            Neurons = neurons;
            Size = size;
            WeightRange = weightRange;
            ActivationFunction = Math.Tanh;
            InputSize = inputSize;
        }
        public Layer(int size, int inputSize, STuple<double, double> weightRange, Func<double, double> activateWith, bool init = true)
        {
            Size = size;
            WeightRange = weightRange;
            ActivationFunction = activateWith;
            InputSize = inputSize;
            Neurons = new INeuron[size];
        }

        public void Create()
        {
            for (int i = 0; i < Neurons.Length; i++)
            {
                var neuron = new Neuron(InputSize, WeightRange, ActivationFunction);
                neuron.Create();
                Neurons[i] = neuron;
            }
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
