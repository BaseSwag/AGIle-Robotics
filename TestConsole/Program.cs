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
                popSize: (10, 20),
                ports: (3, 1),
                length: (2, 8),
                width: (1, 5),
                weightRange: (-2.0, 2.0),
                activateWith: Math.Tanh
                );

            ReportStatus(Task).Wait();

            Trainer.ActivationType = Trainer.TrainerActivationType.Single;
            Trainer.SetFitnessFunction(new Func<INeuralNetwork, Task<double>>(FitnessFunction));

            for (int i = 0; i < 1000; i++)
            {
                Task = Trainer.EvaluateAndEvolve();
                ReportStatus(Task).Wait();
            }

            Console.WriteLine();
            FitnessFunction(Trainer.Best, true);

            string json = Trainer.Best.Serialize();
            System.IO.File.WriteAllText(@"C:\Users\login\Desktop\network.json", json);

            Console.ReadLine();
            Console.WriteLine();


        }

        static async Task ReportStatus(Task task, bool anyway = false)
        {
            while (!task.IsCompleted || anyway)
            {
                Console.Clear();
                Console.WriteLine($"Transition ratio: {Trainer.StatusUpdater.TransitionRatio}");
                Console.WriteLine($"Random ratio: {Trainer.StatusUpdater.RandomRatio}");
                Console.WriteLine($"Mutation ratio: {Trainer.StatusUpdater.MutationRatio}");
                Console.WriteLine($"Creation ratio: {Trainer.StatusUpdater.CreationRatio}");
                Console.WriteLine($"Population count: {Trainer.StatusUpdater.PopulationCount}");
                Console.WriteLine($"Network count: {Trainer.StatusUpdater.NetworkCount}");
                Console.WriteLine($"Level: {Trainer.StatusUpdater.GenerationLevel}");
                Console.WriteLine($"Activity: {Trainer.StatusUpdater.Activity}");
                Console.WriteLine($"Best fitness: {Trainer.StatusUpdater.BestFitness}");
                Console.WriteLine($"Evaluations running: {Trainer.StatusUpdater.EvaluationsRunning}");
                Console.WriteLine($"Networks evolved: {Trainer.StatusUpdater.NetworksEvolved}");
                await Task.Delay(500);
            }
        }

        static Random random = new Random();
        static Task<double> FitnessFunction(INeuralNetwork network) => FitnessFunction(network, false);
        static Task<double> FitnessFunction(INeuralNetwork network, bool debug)
        {
            double fitness = 0;

            #region sequence
            /*
            for (int n = 0; n < 10; n++)
            {
                Random random = new Random();
                var options = Enumerable.Range(0, Amount).ToList();
                var test = new double[Amount];
                for (int i = 0; i < Amount; i++)
                {
                    int r = random.Next(options.Count);
                    var next = (double)options[r];
                    next = Extensions.Map(next, 0, (double)Amount, -1, 1);
                    test[i] = next;
                    options.RemoveAt(r);
                }
                if (test.Length != Amount)
                {
                    Console.WriteLine("wtf!");
                }
                var result = network.Activate(test);
                for (int i = 0; i < Amount; i++)
                {
                    double loss = Math.Abs(result[i] - test[i]);
                    fitness -= loss;
                    if (debug && n == 0)
                        Console.WriteLine($"{test[i]} | {result[i]}");
                }
            }
            */
            #endregion
            #region XOR
            /*
            var tests = new Dictionary<double[], double>
            {
                {
                    new double[] { 0, 0},
                    0
                },
                {
                    new double[] { 1, 0},
                    1
                },
                {
                    new double[] { 0, 1},
                    1
                },
                {
                    new double[] { 1, 1},
                    0
                },
            };

            foreach(var kv in tests)
            {
                var result = network.Activate(kv.Key);
                double difference = result[0] - kv.Value;
                difference = -Math.Abs(difference);
                fitness += difference;
                if (debug)
                {
                    Console.WriteLine($"{kv.Key[0]} {kv.Key[1]} | {result[0]}");
                }
            }
            */
            #endregion XOR
            #region linear
            for (int i = 0; i < 10; i++)
            {
                double a, b, x;
                lock (random)
                {
                    a = Math.Round(random.NextDouble() / 2, 2);
                    b = Math.Round(random.NextDouble() / 2, 2);
                    x = Math.Round(random.NextDouble(), 2);
                }
                var res = network.Activate(new double[] { a, b, x });
                var correct = (a * x + b);

                fitness -= Math.Abs(res[0] - correct);

                if (debug)
                {
                    Console.WriteLine($"{a} * {x} + {b} = {correct} | {res[0]}");
                }
            }
            #endregion

            var tcs = new TaskCompletionSource<double>();
            tcs.SetResult(fitness);
            return tcs.Task;
        }
    }
}
