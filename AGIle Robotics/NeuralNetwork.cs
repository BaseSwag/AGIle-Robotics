using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AGIle_Robotics.Interfaces;

namespace AGIle_Robotics
{
    public class NeuralNetwork : INeuralNetwork
    {
        public ILayer[] Layers { get => layers; private set => layers = value; }
        private ILayer[] layers;

        public int InputSize => Layers.Length > 0 ? Layers[0].Neurons.Length : 0;

        public int OutputSize => Layers.Length > 0 ? Layers[Layers.Length - 1].Neurons.Length : 0;

        public NeuralNetwork(int[] definition, Func<double, double> activateWith)
        {
            Layers = new ILayer[definition.Length];
            for(int i = 0; i < definition.Length; i++)
            {
                int inputSize = i > 0 ? definition[i - 1] : definition[i];
                ILayer layer = new Layer(definition[i], inputSize, activateWith);
                Layers[i] = layer;
            }
        }

        public double[] Activate(double[] input)
        {
            for (int i = 0; i < Layers.Length; i++)
            {
                input = Layers[i].Activate(input);
            }
            return input;
        }
    }
}
