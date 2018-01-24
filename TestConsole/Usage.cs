using AGIle_Robotics;
using AGIle_Robotics.Interfaces;
using SuperTuple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    public static class Usage
    {
        static Trainer Trainer;
        static Queue<Fight> Fights = new Queue<Fight>();

        public static async Task Init()
        {
            Trainer = new Trainer(
                transitionRatio: 0.5,
                randomRatio: 0.1,
                mutationRatio: 0.1);

            await Trainer.Initialize(
                size: 20,
                popSize: (10, 20),
                ports: (1, 1),
                length: (5, 15),
                width: (2, 15),
                weightRange: (-2.0, 2.0),
                activateWith: Math.Tanh
                );

            await Trainer.Create();

            Trainer.FitnessFunction = FitnessFunction;
        }

        public static async Task EvaluateAndEvolve(int a)
        {
            for(int i = 0; i < a; i++)
            {
                await Trainer.EvaluateAndEvolve();
            }
        }

        static Task<STuple<double, double>> FitnessFunction (INeuralNetwork n1, INeuralNetwork n2) 
        {
            TaskCompletionSource<STuple<double, double>> tcs = new TaskCompletionSource<STuple<double, double>>();
            var fight = new Fight(n1, n2, tcs);
            lock (Fights)
            {
                Fights.Enqueue(fight);
            }
            return tcs.Task;
        }
    }
}
