﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGIle_Robotics.Interfaces
{
    public interface INeuron : INeuralElement
    {
        double[] InputWeights { get; set; }
    }
}
