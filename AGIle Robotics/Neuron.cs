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
    public class Neuron : INeuron
    {
        [JsonConverter(typeof(ArrayListJsonConverter<double>))]
        public double[] InputWeights { get => inputWeights; set => inputWeights = value; }
        private double[] inputWeights;

        public int InputSize { get => inputSize; set => inputSize = value; }
        private int inputSize;

        [JsonIgnore]
        public Func<double, double> ActivationFunction { get => activationFunction; private set => activationFunction = value; }
        private Func<double, double> activationFunction = Math.Tanh;

        [JsonConverter(typeof(DoubleTupleJsonConverter))]
        public (double, double) WeightRange { get => weightRange; set => weightRange = value; }
        private (double, double) weightRange;

        [JsonConstructor]
        public Neuron(double[] inputWeights, int inputSize, STuple<double, double> weightRange)
        {
            InputWeights = inputWeights;
            InputSize = inputSize;

            WeightRange = weightRange;
            ActivationFunction = Math.Tanh;
        }
        public Neuron(int inputSize, STuple<double, double> weightRange, Func<double, double> activateWith, bool init = true)
        {
            InputSize = inputSize;
            WeightRange = weightRange;
            ActivationFunction = activateWith;

            InputWeights = new double[inputSize];
            if (init)
            {
                for(int i = 0; i < InputWeights.Length; i++)
                {
                    InputWeights[i] = Extensions.RandomDouble(weightRange.Item1, weightRange.Item2);
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
            var neuron2 = e as Neuron;
            int len = InputWeights.Length;

            if(len == neuron2?.InputWeights.Length)
            {
                var newNeuron = new Neuron(len, WeightRange, ActivationFunction, false);
                for(int i = 0; i < len; i++)
                {
                    var decision = Extensions.DecideByProbability(p1, p2);
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
            for (int i = 0; i < InputWeights.Length; i++)
            {
                if (Extensions.RandomBool(ratio))
                {
                    double rand = InputWeights[i];
                    var absRatio = Math.Abs(ratio);
                    rand += Extensions.RandomDouble(-absRatio, absRatio);
                    rand = Extensions.Cap(rand, WeightRange);
                    InputWeights[i] = rand;
                }
            };
        }
    }
}
