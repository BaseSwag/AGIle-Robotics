using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AGIle_Robotics.Interfaces;

namespace AGIle_Robotics
{
    public class Layer : ILayer
    {
        public INeuron[] Neurons { get => neurons; private set => neurons = value; }
        private INeuron[] neurons;

        public Tuple<double, double> WeightRange { get => weightRange; private set => weightRange = value; }
        private Tuple<double, double> weightRange;

        public Func<double, double> ActivationFunction { get => activationFunction; private set => activationFunction = value; }
        private Func<double, double> activationFunction;

        private int inputSize;

        public Layer(int size, int inputSize, Tuple<double, double> weightRange, Func<double, double> activateWith, bool init = true)
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

        public double[] Activate(double[] input)
        {
            double[] output = new double[Neurons.Length];
            for(int i = 0; i < Neurons.Length; i++)
            {
                output[i] = Neurons[i].Activate(input)[0];
            }
            return output;
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
            {
                Neurons[i].Mutate(ratio);
            }
        }
    }
}
