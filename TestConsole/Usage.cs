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
        public static int FightsLeft = 0;

        public static async Task Init()
        {
            Trainer = new Trainer(
                transitionRatio: 0.5,
                randomRatio: 0.1,
                mutationRatio: 0.1);

            await Trainer.Initialize(
                size: 10,
                popSize: (10, 15),
                ports: (1, 1),
                length: (5, 15),
                width: (2, 10),
                weightRange: (-2.0, 2.0),
                activateWith: Math.Tanh
                );

            await Trainer.Create();

            Trainer.FitnessFunction = FitnessFunction;
        }

        public static async Task EvaluateAndEvolve(int a = 1)
        {
            for(int i = 0; i < a; i++)
            {
                Console.WriteLine(Trainer.Level);
                await Trainer.EvaluateAndEvolve();
            }
        }

        static async Task<STuple<double, double>> FitnessFunction (INeuralNetwork n1, INeuralNetwork n2) 
        {
            await Task.Delay(1000);
            Random r = new Random();
            var result = (r.NextDouble(), r.NextDouble());
            return result;
        }
    }
}
