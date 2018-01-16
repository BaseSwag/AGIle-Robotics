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

        public Layer(int size, int inputSize, Tuple<double, double> weightRange, Func<double, double> activateWith)
        {
            Neurons = new INeuron[size];
            for(int i = 0; i < Neurons.Length; i++)
            {
                Neurons[i] = new Neuron(inputSize, weightRange, activateWith);
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
    }
}
