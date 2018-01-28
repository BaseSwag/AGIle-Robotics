using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AGIle_Robotics.Interfaces;
using SuperTuple;

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

        public Neuron(int inputSize, STuple<double, double> weightRange, Func<double, double> activateWith, bool init = true)
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

        public Task<double[]> ActivateAsync(double[] input) => Task.Run(() => Activate(input));

        public INeuralElement CrossOver(INeuralElement e, double p1, double p2)
        {
            var t = CrossOverAsync(e, p1, p2);
            t.Wait();
            return t.Result;
        }
        public async Task<INeuralElement> CrossOverAsync(INeuralElement e, double p1, double p2)
        {
            var neuron2 = e as Neuron;
            int len = InputWeights.Length;

            if(len == neuron2?.InputWeights.Length)
            {
                var newNeuron = new Neuron(len, WeightRange, ActivationFunction, false);
                await Environment.TaskForAsync(0, len, i =>
                {
                    var decision = Environment.DecideByProbability(p1, p2);
                    newNeuron.InputWeights[i] = decision ? neuron2.InputWeights[i] : InputWeights[i];
                });
                return newNeuron;
            }
            else
            {
                throw new InvalidOperationException("Cannot perform CrossOver on Neurons with different InputWeights sizes.");
            }
        }

        public void Mutate(double ratio) => MutateAsync(ratio).Wait();
        public Task MutateAsync(double ratio)
        {
            return Environment.TaskForAsync(0, InputWeights.Length, index =>
            {
                if (Environment.RandomBool(ratio))
                {
                    double rand = InputWeights[index];
                    var absRatio = Math.Abs(ratio);
                    rand += Environment.RandomDouble(-absRatio, absRatio);
                    rand = Environment.Cap(rand, WeightRange);
                    InputWeights[index] = rand;
                }
            });
        }
    }
}
