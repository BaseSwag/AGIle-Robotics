using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGIle_Robotics.Interfaces
{
    public interface INeuralElement : IElement
    {
        double[] Activate(double[] input);
        INeuralElement CrossOver(INeuralElement e, double p1, double p2);
        void Mutate(double ratio);
    }
}
