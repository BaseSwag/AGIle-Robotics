﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGIle_Robotics
{
    interface INeuron : INeuralElement
    {
        double[] InputWeights { get; set; }
        Func<double,double> ActivationFunction { get; }
    }
}
