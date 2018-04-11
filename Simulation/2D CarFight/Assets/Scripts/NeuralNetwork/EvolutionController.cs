using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using AGIle_Robotics.Interfaces;
using AGIle_Robotics;
using SuperTuple;
using System;
using System.Timers;
using System.IO;
using AGIle_Robotics.Extension;



public class EvolutionController : MonoBehaviour
{
    public int fightCountPerRow = 10;
    public int fightRows = 2;
    public GameObject fightPrefab;
    public FightController[] fights = new FightController[10];
    public Queue<Fight> fightsQueue = new Queue<Fight>();

    public static int fightsDone = 0;

    public Task Task;
    public bool initialized = false;
    public volatile bool needsRechecking = false;
    public int timeScale = 1;

    public Text InfoText;

    private Timer timer;
    private DateTime start;

    public Trainer trainer;

    private bool writeOut = false;

    public void CheckForNewFight()
    {
        Debug.Log("Checking for Fight");
        needsRechecking = false;
        int fightIndex = -1;
        for (int i = 0; i < fights.Length; i++)
        {
            if (fights[i].Fight == null || fights[i].Fight.tcs.Task.IsCompleted || fights[i].Fight.tcs.Task.IsFaulted || fights[i].Fight.tcs.Task.IsCanceled)
            {
                fightIndex = i;
                break;
            }
        }

        if (fightIndex > -1 && fightsQueue.Count > 0)
        {
            lock (fightsQueue)
                fights[fightIndex].Fight = fightsQueue.Dequeue();
        }

    }


    public Task<STuple<double, double>> fitnessFunction(INeuralNetwork n1, INeuralNetwork n2)
    {
        TaskCompletionSource<STuple<double, double>> tcs = new TaskCompletionSource<STuple<double, double>>();
        Fight fight = new Fight(n1, n2, tcs);
        lock (fightsQueue)
        {
            fightsQueue.Enqueue(fight);
        }
        fight.tcs.Task.ContinueWith(t =>
        {
            Debug.Log("Continuing");
            needsRechecking = true;
        });
        needsRechecking = true;
        return tcs.Task;

    }

    private void OnGUI()
    {
        lock (fightsQueue)
            InfoText.text = $"Generation: {trainer.Level}\n" + "" +
                $"Fight processed: {fightsDone}\n" +
                $"Fight queue count: {fightsQueue.Count}\n" +
                $"Evaluations running: {trainer.StatusUpdater.EvaluationsRunning}\n" +
                $"Evaluations left: {trainer.StatusUpdater.EvaluationsLeft}\n" +
                $"Networks evolved: {trainer.StatusUpdater.NetworksEvolved}\n" +
                $"Network count: {trainer.StatusUpdater.NetworkCount}\n" +
                $"Population count: {trainer.StatusUpdater.PopulationCount}\n" +
                $"Current activity: {trainer.StatusUpdater.Activity.ToString()}\n" +
                $"Best fitness: {trainer.StatusUpdater.BestFitness}\n";
    }

    void ConsoleTick(object obj, ElapsedEventArgs e)
    {
        writeOut = true;
    }

    // Use this for initialization
    void Start()
    {
        start = DateTime.Now;
        timer = new Timer();
        timer.Elapsed += ConsoleTick;
        timer.Interval = 1000 * 30;

        timer.Start();
        fights = new FightController[fightRows * fightCountPerRow];
        for (int r = 0; r < fightRows; r++)
            for (int i = 0; i < fightCountPerRow; i++)
            {
                GameObject arena = (GameObject)Instantiate(fightPrefab, new Vector3(4000 * i, 4000 * r, 0), new Quaternion(), this.transform);
                FightController controller = arena.GetComponent<FightController>();
                fights[r * fightCountPerRow + i] = controller;
                arena.name = $"Arena {r * fightCountPerRow + i}";
            }

        if (Camera.main != null)
            Camera.main.GetComponent<SmartCamera>().targets = fights.Select((x) => x.gameObject.transform).ToArray();


        Debug.Log("Arenas created");


        Extensions.WorkPool.MaxThreads = fights.Length + 10;

        if (File.Exists("trainer.json"))
        {
            trainer = Trainer.Deserialize(File.ReadAllText("trainer.json"));
            initialized = true;
            Debug.Log("Trainer object loaded");
        }
        else
        {
            trainer = new Trainer(
            transitionRatio: 0.5,
            randomRatio: 0.1,
            mutationRatio: 0.1,
            creationRatio: 0.1
            );

            Debug.Log("Trainer object created");

            Task = Task.Run(async () =>
            {
                await trainer.Initialize(

                size: 10,
                popSize: new Tuple<int, int>(10, 11),
                ports: new Tuple<int, int>(6, 3),
                length: new Tuple<int, int>(5, 15),
                width: new Tuple<int, int>(2, 10),
                weightRange: new Tuple<double, double>(-2, 2),
                activateWith: Math.Tanh
                );
                Debug.Log("Initialized");
                await trainer.Create();
                Debug.Log("Created");
                initialized = true;
            });
        }

        trainer.ActivationType = Trainer.TrainerActivationType.Pair;
        trainer.SetFitnessFunction(new Func<INeuralNetwork, INeuralNetwork, Task<STuple<double, double>>>(fitnessFunction));



    }


    // FixedUpdate is called once per frame
    void FixedUpdate()
    {

        if (initialized && (Task == null || Task.IsCompleted) && trainer.StatusUpdater.EvaluationsLeft == 0)
        {
            Debug.Log("New Generation");
            File.WriteAllText("trainer.json", trainer.Serialize());
            Task = trainer.EvaluateAndEvolve();
        }

        if (needsRechecking)
        {
            int count = 0;
            while (canAssignNewFight() && count < fightRows * fightCountPerRow)
            {
                count++;
                CheckForNewFight();
            }
        }
    }

    bool canAssignNewFight()
    {
        for (int i = 0; i < fights.Length; i++)
        {
            if (fights[i].Fight == null || fights[i].Fight.tcs.Task.IsCompleted || fights[i].Fight.tcs.Task.IsFaulted || fights[i].Fight.tcs.Task.IsCanceled)
                return true;
        }
        return false;
    }
}
