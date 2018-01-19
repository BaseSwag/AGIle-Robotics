﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AGIle_Robotics.Interfaces;

namespace AGIle_Robotics
{
    public class Generation : IGeneration
    {
        public IPopulation[] Populations { get => populations; set => populations = value; }
        private IPopulation[] populations;

        public int Level { get => level; private set => level = value; }
        private int level;

        public int Size { get => size; private set => size = value; }
        private int size;

        public double TransitionRatio { get => transitionRatio; set => transitionRatio = value; }
        private double transitionRatio = 0.5;

        public double RandomRatio { get => randomRatio; set => randomRatio = value; }
        private double randomRatio = 0.1;

        public double MutationRatio { get => mutationRatio; set => mutationRatio = value; }
        private double mutationRatio = 0.1;

        public INeuralNetwork Best { get => best; private set => best = value; }
        public INeuralNetwork best;

        public (int, int) Ports { get => ports; private set => ports = value; }
        public (int, int) ports = (4, 2);

        public (int, int) PopulationSize { get => populationSize; private set => populationSize = value; }
        public (int, int) populationSize = (10, 20);

        public (double, double) WeightRange { get => weightRange; private set => weightRange = value; }
        private (double, double) weightRange = (-2, 2);

        public Func<double, double> ActivationFunction { get => activationFunction; private set => activationFunction = value; }
        private Func<double, double> activationFunction = Math.Tanh;

        public (int, int) Length { get => length; private set => length = value; }
        public (int, int) length = (5, 10);

        public (int, int) Width { get => width; private set => width = value; }


        public (int, int) width = (5, 10);

        public Generation(int size, (int, int) popSize, (int, int) ports, (int, int) length, (int, int) width, (double, double) weightRange)
            => Init(size, popSize, ports, length, width, weightRange, Math.Tanh);
        public Generation(int size, (int, int) popSize, (int, int) ports, (int, int) length, (int, int) width, (double, double) weightRange, Func<double, double> activateWith)
            => Init(size, popSize, ports, length, width, weightRange, activateWith);
        private void Init(int size, (int, int) popSize, (int, int) ports, (int, int) length, (int, int) width, (double, double) weightRange, Func<double, double> activateWith)
        {
            Size = size;
            PopulationSize = popSize;
            Ports = ports;

            Length = length;
            Width = width;

            WeightRange = weightRange;

            ActivationFunction = activateWith;
        }

        public async Task Create()
        {
            Task<IPopulation>[] tasks = new Task<IPopulation>[Size];

            for(int i = 0; i < Size; i++)
            {
                var t = new Task<IPopulation>(() => CreatePopulation());
                tasks[i] = t;

                Environment.WorkPool.EnqueueTask(t);
            }

            Populations = await Task.WhenAll(tasks);
        }
        private IPopulation CreatePopulation()
        {
            var size = Environment.RandomInt(PopulationSize);
            var length = Environment.RandomInt(Length);
            var definition = new int[length];

            definition[0] = Ports.Item1;
            definition[length - 1] = Ports.Item2;
            for(int i = 1; i < definition.Length - 1; i++)
            {
                var width = Environment.RandomInt(Width);
                definition[i] = width;
            }

            var newPop = new Population(size, definition, WeightRange, ActivationFunction);
            return newPop;
        }

        public void Evaluate(Func<(double, double), (INeuralNetwork, INeuralNetwork)> fitnessFunction)
        {
            throw new NotImplementedException();
        }

        public async Task<IGeneration> Evolve()
        {
            return (IGeneration) await Evolve(TransitionRatio, RandomRatio, MutationRatio);
        }
        public async Task<IEvolvable> Evolve(double transitionRatio, double randomRatio, double mutationRatio)
        {
            Task<IPopulation>[] tasks = new Task<IPopulation>[Size];

            for(int i = 0; i < Size; i++)
            {
                var x = i;
                var t = new Task<IPopulation>(
                    () => (IPopulation)Populations[x].Evolve(TransitionRatio, RandomRatio, MutationRatio));
                tasks[i] = t;

                Environment.WorkPool.EnqueueTask(t);
            }

            Generation newGen = new Generation(Size, PopulationSize, Ports, Length, Width, WeightRange);
            newGen.Populations = await Task.WhenAll(tasks);

            return newGen;
        }
    }
}
