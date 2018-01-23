﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AGIle_Robotics.Interfaces;
using SuperTuple;

namespace AGIle_Robotics
{
    public class NeuralNetwork : INeuralNetwork
    {
        public ILayer[] Layers { get => layers; private set => layers = value; }
        private ILayer[] layers;

        public int InputSize => Layers.Length > 0 ? Layers[0].Neurons.Length : 0;

        public int OutputSize => Layers.Length > 0 ? Layers[Layers.Length - 1].Neurons.Length : 0;

        public (double, double) WeightRange { get => weightRange; private set => weightRange = value; }
        private (double, double) weightRange;

        public double Fitness { get => fitness; set => fitness = value; }
        private double fitness;

        public Func<double, double> ActivationFunction { get => activationFunction; set => activationFunction = value; }
        private Func<double, double> activationFunction;

        public int[] Definition { get => definition; set => definition = value; }
        private int[] definition;

        public NeuralNetwork(int[] definition, STuple<double, double> weightRange, Func<double, double> activateWith, bool init = true)
        {
            WeightRange = weightRange;
            Definition = definition;
            ActivationFunction = activateWith;

            Layers = new ILayer[definition.Length];
            for(int i = 0; i < definition.Length; i++)
            {
                int inputSize = i > 0 ? definition[i - 1] : definition[i];
                ILayer layer = new Layer(definition[i], inputSize, weightRange, activateWith, init);
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
            {
                Layers[i].Mutate(ratio);
            }
        }
    }
}
