using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGIle_Robotics
{
    interface INeuralElement
    {
        double[] Activate(double[] input);
    }
}
