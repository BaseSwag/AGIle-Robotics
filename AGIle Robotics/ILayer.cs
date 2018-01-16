﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGIle_Robotics
{
    public interface ILayer : INeuralElement
    {
        INeuron[] Neurons { get; }
    }
}
