using AGIle_Robotics.Interfaces;
using SuperTuple;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    public class Fight
    {
        public INeuralNetwork Network1;
        public INeuralNetwork Network2;
        public TaskCompletionSource<STuple<double, double>> Tcs;

        public Fight(INeuralNetwork network1, INeuralNetwork network2, TaskCompletionSource<STuple<double, double>> tcs)
        {
            this.Network1 = network1;
            this.Network2 = network2;
            this.Tcs = tcs;
        }
    }
}
