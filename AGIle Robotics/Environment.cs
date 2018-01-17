using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGIle_Robotics
{
    public static class Environment
    {
        public static readonly Func<double, double> DefaultActivationFunction;

        public static Random RNG = new Random();

        public static int RandomInt(int min, int max) => RNG.Next(min, max);
        public static double RandomDouble(double min = 0, double max = 1)
        {
            double rand = RNG.NextDouble();
            rand = Map(rand, 0, 1, min, max);
            return rand;
        }

        public static dynamic Map(dynamic x, dynamic in_min, dynamic in_max, dynamic out_min, dynamic out_max)
        {
            return (x - in_min) * (out_max - out_min) / (in_max - in_min) + out_min;
        }

        public static bool DecideByProbability(double p1, double p2)
        {
            var probability = p1 / (p1 + p2);
            var rand = RandomDouble();
            return rand >= probability;
        }
    }
}
