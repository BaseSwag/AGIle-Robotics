using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGIle_Robotics.Interfaces
{
    public interface IGeneration : IEvolvable
    {
        IPopulation[] Populations { get; }
        int Level { get; }
        int Size { get; }
        double TransitionRatio { get; set; }
        double RandomRatio { get; set; }
        double MutationRatio { get; set; }
        Tuple<int, int> PopulationSize { get; }
        Tuple<int, int> Length { get; }
        Tuple<int, int> Width { get; }
        Tuple<int, int> Ports { get; }
        Task Create();
        Task<IGeneration> Evolve();
        void Evaluate(Func<INeuralNetwork, INeuralNetwork, Task<Tuple<double, double>>> fitnessFunction);
    }
}
