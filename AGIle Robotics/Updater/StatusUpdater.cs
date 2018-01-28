﻿using AGIle_Robotics.Extension;
using System;
using System.Collections.Generic;
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
            Evolving
        }
        public FrameworkActivity Activity { get => GetProperty(ref activity); set => SetProperty(ref activity, value); }
        private FrameworkActivity activity = FrameworkActivity.NotReady;

        public int GenerationLevel { get => GetProperty(ref generationLevel); set => SetProperty(ref generationLevel, value); }
        private int generationLevel;

        public int EvaluationsRunning { get => GetProperty(ref evaluationsRunning); set => SetProperty(ref evaluationsRunning, value); }
        private int evaluationsRunning;

        public int PopulationCount { get => GetProperty(ref populationCount); set => SetProperty(ref populationCount, value); }
        private int populationCount;

        public int NetworkCount { get => GetProperty(ref networkCount); set => SetProperty(ref networkCount, value); }
        private int networkCount;

        public double BestFitness { get => GetProperty(ref bestFitness); set => SetProperty(ref bestFitness, value); }
        private double bestFitness;

        public LimitedQueue<double> FitnessHistory { get => GetProperty(ref fitnessHistory); set => SetProperty(ref fitnessHistory, value); }
        private LimitedQueue<double> fitnessHistory;

        public StatusUpdater(int historySize = 100)
        {
            FitnessHistory = new LimitedQueue<double>(historySize);
        }
    }
}
