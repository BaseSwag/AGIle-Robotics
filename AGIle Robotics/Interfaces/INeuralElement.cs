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
        Task<double[]> ActivateAsync(double[] input);
        INeuralElement CrossOver(INeuralElement e, double p1, double p2);
        Task<INeuralElement> CrossOverAsync(INeuralElement e, double p1, double p2);
        void Mutate(double ratio);
        Task MutateAsync(double ratio);
    }
}
