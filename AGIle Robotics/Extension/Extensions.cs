using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AGIle_Robotics.Updater;
using SuperTuple;

namespace AGIle_Robotics.Extension
{
    public static class Extensions
    {
        internal static readonly WorkPool WorkPool = new WorkPool(6);

        internal static StatusUpdater StatusUpdater = new StatusUpdater();

        private static Random RNG = new Random();
        public static int RandomInt(STuple<int, int> range) => RandomInt(range.Item1, range.Item2);
        public static int RandomInt(int max) => RandomInt(0, max);
        public static int RandomInt(int min, int max)
        {
            lock (RNG)
            {
                return RNG.Next(min, max);
            }
        }
        public static double RandomDouble(double min = 0, double max = 1)
        {
            double rand;
            lock (RNG)
            {
                rand = RNG.NextDouble();
            }
            if(min != 0 || max != 1)
                rand = Map(rand, 0, 1, min, max);

            return rand;
        }
        public static bool RandomBool(double probability)
        {
            double rand = RandomDouble();
            return rand < probability;
        }

        public static dynamic Map(dynamic x, dynamic in_min, dynamic in_max, dynamic out_min, dynamic out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        public static double Cap(double x, STuple<double, double> caps) => Cap(x, caps.Item1, caps.Item2);
        public static double Cap(double x, double min, double max)
        {
            x = Math.Max(min, x);
            x = Math.Min(max, x);
            return x;
        }

        public static bool DecideByProbability(double p1, double p2)
        {
            var probability = p1 / (p1 + p2);
            var rand = RandomDouble();
            return rand >= probability;
        }

        public static void TaskFor(int fromInclusive, int toExclusive, Action<int> body)
            => TaskForAsync(fromInclusive, toExclusive, body).Wait();
        public static Task TaskForAsync(int fromInclusive, int toExclusive, Action<int> body)
        {
            int count = toExclusive - fromInclusive;
            Task[] tasks = new Task[count];
            int counter = 0;
            for(int i = fromInclusive; i < toExclusive; i++)
            {
                int i2 = i;
                var t = Task.Run(() => body(i2));
                tasks[counter++] = t;
            }
            return Task.WhenAll(tasks);
        }
        public static Task<T[]> TaskForAsync<T>(int fromInclusive, int toExclusive, Func<int, T> body)
        {
            int count = toExclusive - fromInclusive;
            Task<T>[] tasks = new Task<T>[count];
            int counter = 0;
            for(int i = fromInclusive; i < toExclusive; i++)
            {
                int i2 = i;
                var t = Task.Run(() => body(i2));
               tasks[counter++] = t;
            }
            return Task.WhenAll(tasks);
        }

        public static int GaussSum(int n)
        {
            return (int)(n / 2.0 * (n + 1));
        }
    }
}
