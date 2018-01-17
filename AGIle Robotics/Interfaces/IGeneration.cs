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
        int MaxThreads { get; set; }
        double TransitionRatio { get; set; }
        double RandomRatio { get; set; }
        double MutationRatio { get; set; }
        int MinLength { get; }
        int MaxLength { get; }
        int MinWidth { get; }
        int MaxWidth { get; }
        IGeneration Evolve();
        void Evaluate(Func<(double, double), (INeuralNetwork, INeuralNetwork)> fitnessFunction);
    }
}
