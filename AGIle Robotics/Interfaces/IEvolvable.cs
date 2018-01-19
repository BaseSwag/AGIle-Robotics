using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGIle_Robotics.Interfaces
{
    public interface IEvolvable : IElement
    {
        INeuralNetwork Best { get; }
        Task<IEvolvable> Evolve(double transitionRatio, double randomRatio, double mutationRate);
    }
}
