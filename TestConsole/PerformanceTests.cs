using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AGIle_Robotics.Extension;

namespace TestConsole
{
    public static class PerformanceTests
    {
        public static void ListPop()
        {
            int range = 10000;
            List<int> list = new List<int>();
            for (int i = 0; i < range * 10; i++)
            {
                list.Add(i * 3);
            }
            List<int> list2 = list.ToList();

            Stopwatch sw = Stopwatch.StartNew();
            var result = new List<int>();
            for (int i = 0; i < range; i++)
            {
                result.Add(list[0]);
                list.RemoveAt(0);
            }
            Console.WriteLine(sw.ElapsedMilliseconds);

            result.Clear();

            sw = Stopwatch.StartNew();
            result.AddRange(list2.Take(range));
            list2.RemoveRange(0, range);
            Console.WriteLine(sw.ElapsedMilliseconds);
            Console.WriteLine(result.Count);
            sw.Stop();

            Console.ReadLine();
        }

        public static void StackNew()
        {
            WorkPool workPool = new WorkPool(6);
            workPool.For(0, 10, i =>
            {
                int i2 = i;
                workPool.For(0, 10, j =>
                {
                    Console.WriteLine($"{i2} : {j}");
                });
            });
        }
    }
}
