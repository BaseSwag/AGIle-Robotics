using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGIle_Robotics
{
    public class Neuron : INeuron
    {
        public double[] InputWeights { get => inputWeights; set => inputWeights = value; }
        private double[] inputWeights;

        public Func<double, double> ActivationFunction { get => activationFunction; }
        private Func<double, double> activationFunction => Math.Abs;

        public Neuron(int inputSize)
        {
            InputWeights = new double[inputSize];
            for(int i = 0; i < InputWeights.Length; i++)
            {
                InputWeights[i] = Environment.RandomDouble(-1, 1);
            }
        }

        public double[] Activate(double[] input)
        {
            if (input.Length != inputWeights.Length)
            {
                throw new ArgumentOutOfRangeException("input", "Amount of input values does not match length of InputWeights");
            }
            
            double wSum = 0;
            for(int i = 0; i < input.Length; i++) { // For all inputs
                wSum += input[i] * InputWeights[i]; // Multiply by corresponding input weight and add to sum
            }

            double output = ActivationFunction(wSum); // Use activation function
            return new double[] { output };
        }
    }
}
