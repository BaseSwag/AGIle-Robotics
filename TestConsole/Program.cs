﻿using System;
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

            workPool = new WorkPool(8);
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
            }

            Task.WaitAll(tasks);
        }

        static void TestMethod()
        {
            Random r = new Random();
            for (int i = 0; i < 10000000; i++)
            {
                var x = r.NextDouble() * r.NextDouble();
            }
        }
    }
}
