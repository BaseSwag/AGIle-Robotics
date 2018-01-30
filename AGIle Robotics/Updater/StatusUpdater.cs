using AGIle_Robotics.Extension;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGIle_Robotics.Updater
{
    public class StatusUpdater : BaseViewModel
    {
        public enum FrameworkActivity
        {
            NotReady = -1,
            Idle,
            Initializing,
            Creating,
            Evaluating,
            Evolving,
        }
        public FrameworkActivity Activity { get => GetProperty(ref activity); set => SetProperty(ref activity, value); }
        private FrameworkActivity activity = FrameworkActivity.NotReady;

        public double TransitionRatio { get => GetProperty(ref transitionRatio); set => SetProperty(ref transitionRatio, value); }
        private double transitionRatio;

        public double RandomRatio { get => GetProperty(ref randomRatio); set => SetProperty(ref randomRatio, value); }
        private double randomRatio;

        public double MutationRatio { get => GetProperty(ref mutationRatio); set => SetProperty(ref mutationRatio, value); }
        private double mutationRatio;

        public double CreationRatio { get => GetProperty(ref creationRatio); set => SetProperty(ref creationRatio, value); }
        private double creationRatio;

        public int GenerationLevel { get => GetProperty(ref generationLevel); set => SetProperty(ref generationLevel, value); }
        private int generationLevel;

        public int EvaluationsRunning { get => GetProperty(ref evaluationsRunning); set => SetProperty(ref evaluationsRunning, value); }
        private int evaluationsRunning;

        public int PopulationCount
        {
            get => GetProperty(ref populationCount);
            set
            {
                SetProperty(ref populationCount, value);
                while (PopulationFitnesses.Count < value)
                    PopulationFitnesses.Add(0.0);
            }
        }
        private int populationCount;

        public int NetworkCount { get => GetProperty(ref networkCount); set => SetProperty(ref networkCount, value); }
        private int networkCount;

        public double BestFitness
        {
            get => GetProperty(ref bestFitness);
            set
            {
                SetProperty(ref bestFitness, value);
                FitnessHistory.Add(value);
            }
        }
        private double bestFitness;

        public ObservableCollection<double> FitnessHistory { get => GetProperty(ref fitnessHistory); set => SetProperty(ref fitnessHistory, value); }
        private ObservableCollection<double> fitnessHistory;

        public ObservableCollection<double> PopulationFitnesses { get => GetProperty(ref populationFitnesses); set => SetProperty(ref populationFitnesses, value); }
        private ObservableCollection<double> populationFitnesses;

        public StatusUpdater(int historySize = 100)
        {
            FitnessHistory = new ObservableCollection<double>();
            PopulationFitnesses = new ObservableCollection<double>();
        }
    }
}
