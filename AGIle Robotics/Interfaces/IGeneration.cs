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
        (int, int) PopulationSize { get; }
        (int, int) Length { get; }
        (int, int) Width { get; }
        (int, int) Ports { get; }
        Task Create();
        Task<IGeneration> Evolve();
        void Evaluate(Func<(INeuralNetwork, INeuralNetwork), (double, double)> fitnessFunction);
    }
}
