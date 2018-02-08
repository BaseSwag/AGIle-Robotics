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
    public class NeuralNetwork : INeuralNetwork
    {
        [JsonConverter(typeof(ArrayListJsonConverter<ILayer>))]
        public ILayer[] Layers { get => layers; private set => layers = value; }
        private ILayer[] layers;

        public int InputSize { get => inputSize; set => inputSize = value; }
        private int inputSize;

        public int OutputSize { get => outputSize; set => outputSize = value; }
        private int outputSize;

        [JsonConverter(typeof(DoubleTupleJsonConverter))]
        public (double, double) WeightRange { get => weightRange; private set => weightRange = value; }
        [JsonConverter(typeof(DoubleTupleJsonConverter))]
        private (double, double) weightRange;

        public double Fitness
        {
            get { lock (fitnessLock) return fitness; }
            set { lock (fitnessLock) fitness = value; }
        }
        private double fitness;
        private object fitnessLock = new object();

        [JsonIgnore]
        public Func<double, double> ActivationFunction { get => activationFunction; set => activationFunction = value; }
        [JsonIgnore]
        private Func<double, double> activationFunction = Math.Tanh;

        [JsonConverter(typeof(ArrayListJsonConverter<int>))]
        public int[] Definition { get => definition; set => definition = value; }
        private int[] definition;

        [JsonConstructor]
        public NeuralNetwork(ILayer[] layers, int[] definition, STuple<double, double> weightRange, int inputSize, double fitness)
        {
            Layers = layers;
            InputSize = definition[0];
            OutputSize = definition[definition.Length - 1];
            WeightRange = weightRange;
            Fitness = fitness;
            Definition = definition;
            ActivationFunction = Math.Tanh;
        }
        public NeuralNetwork(int[] definition, STuple<double, double> weightRange, int inputSize, Func<double, double> activateWith)
        {
            InputSize = inputSize;
            OutputSize = definition[definition.Length - 1];
            WeightRange = weightRange;
            Definition = definition;
            ActivationFunction = activateWith;

            Layers = new ILayer[definition.Length];
        }

        public void Create()
        {
            for(int i = 0; i < Definition.Length; i++)
            {
                var inputSize = i > 0 ? Definition[i - 1] : InputSize;
                ILayer layer = new Layer(Definition[i], inputSize, WeightRange, ActivationFunction);
                Layers[i] = layer;
                Layers[i].Create();
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

        public async Task<double[]> ActivateAsync(double[] input)
        {
            for (int i = 0; i < Layers.Length; i++)
            {
                input = await Layers[i].ActivateAsync(input);
            }
            return input;
        }

        public INeuralElement CrossOver(INeuralElement e, double p1, double p2)
        {
            var net2 = e as NeuralNetwork;
            int len = Layers.Length;

            if(len == net2?.Layers.Length && InputSize == net2.InputSize && OutputSize == net2.OutputSize)
            {
                var newNetwork = new NeuralNetwork(Definition, WeightRange, inputSize, ActivationFunction);
                for(int i = 0; i < len; i++)
                {
                    newNetwork.Layers[i] = (ILayer)Layers[i].CrossOver(net2.Layers[i], p1, p2);
                }
                return newNetwork;
            }
            else
            {
                throw new InvalidOperationException("Cannot perform CrossOver on Networks with different definitions.");
            }
        }

        public void Mutate(double ratio)
        {
            for(int i = 0; i < Layers.Length; i++)
                Layers[i].Mutate(ratio);
        }

        public string Serialize()
        {
            var serializationSettings = new JsonSerializerSettings();
            serializationSettings.TypeNameHandling = TypeNameHandling.All;

            string json = JsonConvert.SerializeObject(this, Formatting.None, serializationSettings);
            return json;
        }

        public static NeuralNetwork Deserialize(string json)
        {
            var serializationSettings = new JsonSerializerSettings();
            serializationSettings.TypeNameHandling = TypeNameHandling.All;

            return JsonConvert.DeserializeObject<NeuralNetwork>(json, serializationSettings);
        }
    }
}
