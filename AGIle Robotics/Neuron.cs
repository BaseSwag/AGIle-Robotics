using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AGIle_Robotics.Interfaces;

namespace AGIle_Robotics
{
    public class Neuron : INeuron
    {
        public double[] InputWeights { get => inputWeights; set => inputWeights = value; }
        private double[] inputWeights;

        public Func<double, double> ActivationFunction { get => activationFunction; private set => activationFunction = value; }
        public Tuple<double, double> WeightRange { get => weightRange; set => weightRange = value; }
        private Tuple<double, double> weightRange;

        private Func<double, double> activationFunction;

        public Neuron(int inputSize, Tuple<double, double> weightRange, Func<double, double> activateWith)
        {
            WeightRange = weightRange;
            ActivationFunction = activateWith;

            InputWeights = new double[inputSize];
            for(int i = 0; i < InputWeights.Length; i++)
            {
                InputWeights[i] = Environment.RandomDouble(weightRange.Item1, weightRange.Item2);
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
