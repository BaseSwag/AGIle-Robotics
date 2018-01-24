using AGIle_Robotics.Interfaces;
using System.Threading.Tasks;
using System;
using SuperTuple;

public class Fight
{
    public INeuralNetwork n1;
    public INeuralNetwork n2;
    public TaskCompletionSource<STuple<double, double>> tcs;

    public Fight(INeuralNetwork n1, INeuralNetwork n2, TaskCompletionSource<STuple<double, double>> tcs)
    {
        this.n1 = n1;
        this.n2 = n2;
        this.tcs = tcs;
    }
}

