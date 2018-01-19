using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AGIle_Robotics;
using AGIle_Robotics.Interfaces;

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
            var generation = new Generation(
                size: 50,
                popSize: (50, 100),
                ports: (4, 2),
                length: (10, 100),
                width: (10, 50),
                weightRange: (-2, 2));

            generation.Create().Wait();
            generation.Evaluate(fitnessFunction);

            Console.WriteLine();

            for (int i = 0; i < 20; i++)
            {
                int rand = AGIle_Robotics.Environment.RandomInt(generation.Populations.Length);
                var pop = generation.Populations[rand];
                rand = AGIle_Robotics.Environment.RandomInt(pop.Networks.Length);
                var net = pop.Networks[rand];

                double[] input = new double[] { .1, .3, -.5, .75 };
                var result = net.Activate(input);
                foreach (var d in result)
                {
                    Console.Write($"{d} ");
                }
                Console.WriteLine();
            }

            Console.ReadLine();
        }

        static void TestPerformance()
        {
            DateTime start;
            DateTime end;

            Console.WriteLine("Linear");
            start = DateTime.Now;
            for(int i = 0; i < steps; i++)
            {
                Console.WriteLine(i);
                TestMethod();
            }
            end = DateTime.Now;
            Console.WriteLine(end - start);

            Console.WriteLine();

            Console.WriteLine("WorkPool");
            start = DateTime.Now;
            TestPool();
            end = DateTime.Now;
            Console.WriteLine(end - start);

            Console.WriteLine();

            workPool.MaxThreads = 8;
            Console.WriteLine("WorkPool 2");
            start = DateTime.Now;
            TestPool();
            end = DateTime.Now;
            Console.WriteLine(end - start);
            Console.ReadLine();
        }

        static void TestPool()
        {
            for(int i = 0; i < steps; i++)
            {
                Task t = new Task(() => TestMethod());
                tasks[i] = t;

                workPool.EnqueueTask(t);
                //Console.WriteLine($"Workload: {workPool.Workload}");
            }

            Task.WaitAll(tasks);
        }

        static void TestMethod()
        {
            Random r = new Random();
            for (int i = 0; i < 10000000; i++)
            {
                var x = Math.Sqrt(r.NextDouble());
            }
        }

        static Task<Tuple<double, double>> fitnessFunction (INeuralNetwork n1, INeuralNetwork n2) 
        {
            TaskCompletionSource<Tuple<double, double>> tcs = new TaskCompletionSource<Tuple<double, double>>();
            TestMethod();
            return tcs.Task;
        }
    }
}
