using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperTuple;

namespace AGIle_Robotics.Interfaces
{
    public interface ITrainer
    {
        double TransitionRatio { get; set; }
        double RandomRatio { get; set; }
        double MutationRatio { get; set; }
        int Level { get; }
        IGeneration CurrentGeneration { get; }
        Func<INeuralNetwork, INeuralNetwork, Task<STuple<double, double>>> FitnessFunction { get; set; }
        Task Initialize(int size, STuple<int, int> popSize, STuple<int, int> ports, STuple<int, int> length, STuple<int, int> width, STuple<double, double> weightRange, Func<double, double> activateWith);
        Task Create();
        Task InitializeAndCreate(int size, STuple<int, int> popSize, STuple<int, int> ports, STuple<int, int> length, STuple<int, int> width, STuple<double, double> weightRange, Func<double, double> activateWith);
        Task Evolve();
        Task Evaluate();
        Task Evaluate(Func<INeuralNetwork, INeuralNetwork, Task<STuple<double, double>>> fitnessFunction);
        Task EvaluateAndEvolve();
        Task EvaluateAndEvolve(Func<INeuralNetwork, INeuralNetwork, Task<STuple<double, double>>> fitnessFunction);
    }
}
