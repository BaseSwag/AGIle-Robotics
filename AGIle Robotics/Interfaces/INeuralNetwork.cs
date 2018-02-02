using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGIle_Robotics.Interfaces
{
    public interface INeuralNetwork : INeuralElement
    {
        ILayer[] Layers { get; }
        int InputSize { get; }
        int OutputSize { get; }
        int[] Definition { get; }
        double Fitness { get; set; }
        string Serialize();
    }
}
