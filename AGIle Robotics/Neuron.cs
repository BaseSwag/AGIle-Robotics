using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGIle_Robotics
{
    class Neuron : INeuron
    {
        public double[] InputWeights { get => inputWeights; set => inputWeights = value; }
        public Func<double, double> ActivationFunction { get => activationFunction; }
        private Func<double, double> activationFunction => Math.Abs;

        private double[] inputWeights;

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
