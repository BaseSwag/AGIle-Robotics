﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGIle_Robotics.Interfaces
{
    public interface IPopulation : IEvolvable
    {
        INeuralNetwork[] Networks { get; }
        void ResetBest();
        new Task<IPopulation> Evolve(double transitionRatio, double randomRatio, double mutationRatio, double creationRatio);
        int Size { get; }
        int[] Definition { get; }
    }
}
