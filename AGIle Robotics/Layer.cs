using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AGIle_Robotics.Interfaces;
using SuperTuple;

namespace AGIle_Robotics
{
    public class Layer : ILayer
    {
        public INeuron[] Neurons { get => neurons; private set => neurons = value; }
        private INeuron[] neurons;

        public (double, double) WeightRange { get => weightRange; private set => weightRange = value; }
        private (double, double) weightRange;

        public Func<double, double> ActivationFunction { get => activationFunction; private set => activationFunction = value; }
        private Func<double, double> activationFunction;

        private int inputSize;

        public Layer(int size, int inputSize, STuple<double, double> weightRange, Func<double, double> activateWith, bool init = true)
        {
            WeightRange = weightRange;
            ActivationFunction = activateWith;
            this.inputSize = inputSize;

            Neurons = new INeuron[size];
            Environment.TaskFor(0, Neurons.Length, index =>
            {
                Neurons[index] = new Neuron(inputSize, weightRange, activateWith, init);
            });
        }

        public double[] Activate(double[] input)
        {
            var output = new double[Neurons.Length];
            Environment.TaskFor(0, Neurons.Length, index =>
            {
                output[index] = Neurons[index].Activate(input)[0];
            });
            return output;
        }

        public Task<double[]> ActivateAsync(double[] input)
        {
            return Environment.TaskForAsync(0, Neurons.Length, i => Neurons[i].ActivateAsync(input).Result[0]);
            /*
                WorkPool workPool = new WorkPool(Environment.WorkCapacity);
                Task<double>[] tasks = new Task<double>[Neurons.Length];

                for (int i = 0; i < Neurons.Length; i++)
                {
                    var x = i;

                    Task<double> t = new Task<double>(() => Neurons[x].ActivateAsync(input).Result[0]);
                    tasks[i] = t;
                    workPool.EnqueueTask(t);
                }
            */
        }

        public INeuralElement CrossOver(INeuralElement e, double p1, double p2)
        {
            var task = CrossOverAsync(e, p1, p2);
            task.Wait();
            return task.Result;
        }
        public async Task<INeuralElement> CrossOverAsync(INeuralElement e, double p1, double p2)
        {
            var layer2 = e as Layer;
            int len = Neurons.Length;

            if(len == layer2?.Neurons.Length && inputSize == layer2.inputSize)
            {
                var newLayer = new Layer(len, inputSize, WeightRange, ActivationFunction, false);
                await Environment.TaskForAsync(0, len, index =>
                {
                    newLayer.Neurons[index] = (INeuron)Neurons[index].CrossOver(layer2.Neurons[index], p1, p2);
                });
                return newLayer;
            }
            else
            {
                throw new InvalidOperationException("Cannot perform CrossOver on Layers with different inputs.");
            }
        }

        public void Mutate(double ratio) => MutateAsync(ratio).Wait();
        public Task MutateAsync(double ratio)
        {
            return Environment.TaskForAsync(0, Neurons.Length, index =>
                Neurons[index].Mutate(ratio));
        }
    }
}
