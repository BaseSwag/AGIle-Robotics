using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperTuple;

namespace AGIle_Robotics.Interfaces
{
    public interface IGeneration : IEvolvable
    {
        IPopulation[] Populations { get; }
        int Size { get; }
        double TransitionRatio { get; set; }
        double RandomRatio { get; set; }
        double MutationRatio { get; set; }
        double CreationRatio { get; set; }
        (int, int) PopulationSize { get; }
        (int, int) Length { get; }
        (int, int) Width { get; }
        (int, int) Ports { get; }
        Task Create();
        new Task<IGeneration> Evolve(double transitionRatio, double randomRatio, double mutationRatio);
        Task Evaluate(Func<INeuralNetwork, INeuralNetwork, Task<STuple<double, double>>> fitnessFunction);
    }
}
