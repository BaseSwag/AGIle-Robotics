﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SuperTuple;

namespace AGIle_Robotics.Interfaces
{
    public interface IElement
    {
        (double, double) WeightRange { get; }
        Func<double, double> ActivationFunction { get; }
        void Create();
    }
}
