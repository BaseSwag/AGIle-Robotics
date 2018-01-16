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
        static void Main(string[] args)
        {
            INeuron neuron = new Neuron(3, Math.Abs);
            double[] input = new double[] { 1, 0.5, -0.75 };
            var output = neuron.Activate(input);
            foreach (var o in output)
                Console.WriteLine(o);

            Console.ReadLine();
        }
    }
}
