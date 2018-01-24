using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AGIle_Robotics;
using AGIle_Robotics.Interfaces;
using SuperTuple;

namespace TestConsole
{
    class Program
    {
        const int steps = 50;
        static WorkPool workPool = new WorkPool(4);
        static Random random = new Random();
        static Task[] tasks = new Task[steps];

        static int[] proof = new int[steps];

        static void Main(string[] args)
        {
            var trainer = new Trainer(
                transitionRatio: 0.5,
                randomRatio: 0.1,
                mutationRatio: 0.1);

            trainer.Initialize(
                size: 5,
                popSize: (5, 10),
                ports: (1, 1),
                length: (2, 5),
                width: (2, 5),
                weightRange: (-2.0, 2.0),
                activateWith: Math.Tanh
                ).Wait();

            trainer.Create().Wait();

            trainer.FitnessFunction = FitnessFunction;

            for(int i = 0; i < 10000; i++)
            {
                Console.WriteLine($"Generation: {trainer.Level}");
                //trainer.EvaluateAndEvolve(FitnessFunction).Wait();
                trainer.Evaluate().Wait();
                trainer.Evolve().Wait();
            }

            for(int i = 0; i < 1; i++)
            {
                var testTask = trainer.CurrentGeneration.Best.ActivateAsync(new double[] { 1 });
                testTask.Wait();
                var test = testTask.Result[0];

                Console.WriteLine($"{test}");
            }

            Console.WriteLine();
            Console.ReadLine();
        }

        static async Task<double> TestSumFitness(INeuralNetwork n)
        {
            var result = await TestSum(n);
            return result.Item1;
        }
        static async Task<(double, double, double, double)> TestSum(INeuralNetwork n)
        {
            Random r = new Random();
            var a = r.NextDouble();
            var b = r.NextDouble();
            var correct = a + b;

            var result = await n.ActivateAsync(new double[] { a, b });
            double sum = result[0];

            double fitness = correct - sum;
            fitness = -Math.Abs(fitness);
            return (fitness, a, b, sum);
        }

        static async Task<(double, double)> TestLarger(INeuralNetwork n1, INeuralNetwork n2, TaskCompletionSource<STuple<double, double>> tcs)
        {
            var a = await n1.ActivateAsync(new double[] { 1 });
            var b = await n2.ActivateAsync(new double[] { 1 });

            (double, double) result;
            if(a[0] < b[0])
            {
                result = (-1, 1);
                tcs.SetResult(result);
            }
            else
            {
                result = (1, -1);
                tcs.SetResult(result);
            }
            return result;
        }

        static Task<STuple<double, double>> FitnessFunction (INeuralNetwork n1, INeuralNetwork n2) 
        {
            TaskCompletionSource<STuple<double, double>> tcs = new TaskCompletionSource<STuple<double, double>>();
            TestLarger(n1, n2, tcs);
            return tcs.Task;
        }

        static async Task TestFitnessFunction(INeuralNetwork n1, INeuralNetwork n2, TaskCompletionSource<STuple<double, double>> tcs)
        {
            var results = await Task.WhenAll(new Task<double>[] { TestSumFitness(n1), TestSumFitness(n1) });
            tcs.SetResult((results[0], results[1]));
        }
    }
}
