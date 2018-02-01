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

        [JsonProperty]
        public int InputSize => Layers.Length > 0 ? Layers[0].Neurons.Length : 0;

        [JsonProperty]
        public int OutputSize => Layers.Length > 0 ? Layers[Layers.Length - 1].Neurons.Length : 0;

        [JsonConverter(typeof(DoubleTupleJsonConverter))]
        public (double, double) WeightRange { get => weightRange; private set => weightRange = value; }
        [JsonConverter(typeof(DoubleTupleJsonConverter))]
        private (double, double) weightRange;

        public double Fitness { get => fitness; set => fitness = value; }
        private double fitness;

        [JsonIgnore]
        public Func<double, double> ActivationFunction { get => activationFunction; set => activationFunction = value; }
        [JsonIgnore]
        private Func<double, double> activationFunction = Math.Tanh;

        [JsonConverter(typeof(ArrayListJsonConverter<int>))]
        public int[] Definition { get => definition; set => definition = value; }
        private int[] definition;

        [JsonConstructor]
        public NeuralNetwork(ILayer[] layers, int[] definition, STuple<double, double> weightRange, double fitness)
        {
            Layers = layers;

            WeightRange = weightRange;
            Fitness = fitness;
            Definition = definition;
            ActivationFunction = Math.Tanh;
        }
        public NeuralNetwork(int[] definition, STuple<double, double> weightRange, Func<double, double> activateWith, bool init = true)
        {
            WeightRange = weightRange;
            Definition = definition;
            ActivationFunction = activateWith;

            Layers = new ILayer[definition.Length];
            Extensions.WorkPool.For(0, definition.Length, index =>
            {
                int inputSize = index > 0 ? definition[index - 1] : definition[index];
                ILayer layer = new Layer(definition[index], inputSize, weightRange, activateWith, init);
                Layers[index] = layer;
            });
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
                var newNetwork = new NeuralNetwork(Definition, WeightRange, ActivationFunction, false);
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
