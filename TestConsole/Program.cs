using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AGIle_Robotics;
using AGIle_Robotics.Extension;
using AGIle_Robotics.Interfaces;
using SuperTuple;
using Newtonsoft.Json;

namespace TestConsole
{
    class Program
    {
        static Trainer Trainer;
        static Task Task;

        static void Main(string[] args)
        {
            Trainer = new Trainer(
                transitionRatio: 0.5,
                randomRatio: 0.1,
                mutationRatio: 0.1,
                creationRatio: 0.1);

            Task = Trainer.InitializeAndCreate(
                size: 10,
                popSize: (10, 11),
                ports: (6, 3),
                length: (5, 15),
                width: (2, 10),
                weightRange: (-2.0, 2.0),
                activateWith: Math.Tanh
                );

            ReportStatus(Task).Wait();

            Trainer.FitnessFunction = FitnessFunction;

            for (int i = 0; i < 0; i++)
            {
                Task = Trainer.EvaluateAndEvolve();
                ReportStatus(Task).Wait();
            }

            string json = Trainer.Serialize();
            System.IO.File.WriteAllText(@"C:\Users\login\Desktop\trainer.js", json);

            Trainer = Trainer.Deserialize(json);

            Console.ReadLine();
        }

        static async Task ReportStatus(Task task, bool anyway = false)
        {
            while (!task.IsCompleted || anyway)
            {
                Console.Clear();
                Console.WriteLine($"Transition ratio: {Trainer.StatusUpdater.TransitionRatio}");
                Console.WriteLine($"Random ratio: {Trainer.StatusUpdater.RandomRatio}");
                Console.WriteLine($"Mutation ratio: {Trainer.StatusUpdater.MutationRatio}");
                Console.WriteLine($"Population count: {Trainer.StatusUpdater.PopulationCount}");
                Console.WriteLine($"Network count: {Trainer.StatusUpdater.NetworkCount}");
                Console.WriteLine($"Level: {Trainer.StatusUpdater.GenerationLevel}");
                Console.WriteLine($"Activity: {Trainer.StatusUpdater.Activity}");
                Console.WriteLine($"Best fitness: {Trainer.StatusUpdater.BestFitness}");
                Console.WriteLine($"Evaluations running: {Trainer.StatusUpdater.EvaluationsRunning}");
                await Task.Delay(500);
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
