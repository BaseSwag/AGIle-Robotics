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
        private Func<double, double> activationFunction;

        public (double, double) WeightRange { get => weightRange; set => weightRange = value; }
        private (double, double) weightRange;


        public Neuron(int inputSize, (double, double) weightRange, Func<double, double> activateWith, bool init = true)
        {
            WeightRange = weightRange;
            ActivationFunction = activateWith;

            InputWeights = new double[inputSize];
            if (init)
            {
                for(int i = 0; i < InputWeights.Length; i++)
                {
                    InputWeights[i] = Environment.RandomDouble(weightRange.Item1, weightRange.Item2);
                }
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

        public INeuralElement CrossOver(INeuralElement e, double p1, double p2)
        {
            var neuron2 = e as Neuron;
            int len = InputWeights.Length;

            if(len == neuron2?.InputWeights.Length)
            {
                var newNeuron = new Neuron(len, WeightRange, ActivationFunction, false);
                for(int i = 0; i < len; i++)
                {
                    var decision = Environment.DecideByProbability(p1, p2);
                    newNeuron.InputWeights[i] = decision ? neuron2.InputWeights[i] : InputWeights[i];
                }
                return newNeuron;
            }
            else
            {
                throw new InvalidOperationException("Cannot perform CrossOver on Neurons with different InputWeights sizes.");
            }
        }

        public void Mutate(double ratio)
        {
            for(int i = 0; i < InputWeights.Length; i++)
            {
                if (Environment.RandomBool(ratio))
                {
                    double rand = InputWeights[i];
                    rand += Environment.RandomDouble(-Math.Abs(ratio), Math.Abs(ratio));
                    rand = Environment.Cap(rand, WeightRange);
                    InputWeights[i] = rand;
                }
            }
        }
    }
}
