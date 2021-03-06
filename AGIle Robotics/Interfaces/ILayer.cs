﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGIle_Robotics.Interfaces
{
    public interface ILayer : INeuralElement
    {
        int Size { get; }
        int InputSize { get; }
        INeuron[] Neurons { get; }
    }
}
