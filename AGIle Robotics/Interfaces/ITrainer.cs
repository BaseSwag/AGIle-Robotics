using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGIle_Robotics.Interfaces
{
    public interface ITrainer
    {
        double TransitionRatio { get; set; }
        double RandomRatio { get; set; }
        double MutationRatio { get; set; }
        IGeneration CurrentGeneration { get; }
        Func<INeuralNetwork, INeuralNetwork, Task<Tuple<double, double>>> FitnessFunction { get; set; }
        Task Initialize(int size, Tuple<int, int> popSize, Tuple<int, int> ports, Tuple<int, int> length, Tuple<int, int> width, Tuple<double, double> weightRange, Func<double, double> activateWith);
        Task Create();
        Task Evolve();
        Task Evaluate();
        Task Evaluate(Func<INeuralNetwork, INeuralNetwork, Task<Tuple<double, double>>> fitnessFunction);
    }
}
