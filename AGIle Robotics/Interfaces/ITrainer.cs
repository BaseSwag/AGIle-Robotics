using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AGIle_Robotics.Updater;
using SuperTuple;

namespace AGIle_Robotics.Interfaces
{
    public interface ITrainer
    {
        StatusUpdater StatusUpdater { get; }
        double TransitionRatio { get; set; }
        double RandomRatio { get; set; }
        double MutationRatio { get; set; }
        double CreationRatio { get; set; }
        int Level { get; }
        Trainer.TrainerActivationType ActivationType { get; set; }
        INeuralNetwork Best { get; }
        IGeneration CurrentGeneration { get; }
        Func<INeuralNetwork, Task<double>> SingleFitnessFunction { get; }
        Func<INeuralNetwork, INeuralNetwork, Task<STuple<double, double>>> PairFitnessFunction { get; }
        Task Initialize(int size, STuple<int, int> popSize, STuple<int, int> ports, STuple<int, int> length, STuple<int, int> width, STuple<double, double> weightRange, Func<double, double> activateWith);
        Task Create();
        Task InitializeAndCreate(int size, STuple<int, int> popSize, STuple<int, int> ports, STuple<int, int> length, STuple<int, int> width, STuple<double, double> weightRange, Func<double, double> activateWith);
        Task Evolve();
        Task Evaluate();
        Task EvaluateAndEvolve();
        void SetFitnessFunction(Delegate function);
        string Serialize();
    }
}
