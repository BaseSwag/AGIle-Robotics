﻿using System;
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
        Task CreateAsync();
        new Task<IGeneration> Evolve(double transitionRatio, double randomRatio, double mutationRatio, double creationRatio);
        Task EvaluateSingle(Func<INeuralNetwork, Task<double>> fitnessFunction);
        Task EvaluatePair(Func<INeuralNetwork, INeuralNetwork, Task<STuple<double, double>>> fitnessFunction);
    }
}
