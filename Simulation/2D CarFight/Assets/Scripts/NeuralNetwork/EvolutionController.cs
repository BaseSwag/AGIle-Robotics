using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using AGIle_Robotics.Interfaces;
using AGIle_Robotics;
using SuperTuple;
using System;



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


    public Trainer trainer;

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
        lock(fightsQueue)
        GUI.Label(new Rect(0, 0, 500, 500), $"Generation: {trainer.Level}\nFight processed: {fightsDone}\nFight queue count: {fightsQueue.Count}");
        timeScale = (int)GUI.HorizontalSlider(new Rect(1, 100, 500, 30), timeScale, 0, 100);
    }

    // Use this for initialization
    void Start()
    {
        Time.timeScale = timeScale;
        fights = new FightController[fightRows * fightCountPerRow];
        for (int r = 0; r < fightRows; r++)
            for (int i = 0; i < fightCountPerRow; i++)
            {
                GameObject arena = (GameObject)Instantiate(fightPrefab, new Vector3(4000 * i, 4000 * r, 0), new Quaternion(), this.transform);
                FightController controller = arena.GetComponent<FightController>();
                fights[r * fightCountPerRow + i] = controller;
                arena.name = $"Arena {r * fightCountPerRow + i}";
            }

        if(Camera.main != null)
        Camera.main.GetComponent<SmartCamera>().targets = fights.Select((x) => x.gameObject.transform).ToArray();


        Debug.Log("Arenas created");

        trainer = new Trainer(
            transitionRatio: 0.5,
            randomRatio: 0.1,
            mutationRatio: 0.1
            );

        trainer.FitnessFunction = fitnessFunction;

        Debug.Log("Trainer object created");

        Task = Task.Run(async () =>
        {
            await trainer.Initialize(
            size: 10,
            popSize: new Tuple<int, int>(10, 20),
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

    // FixedUpdate is called once per frame
    void FixedUpdate()
    {
        if (Time.timeScale != timeScale)
            Time.timeScale = timeScale;

        if (initialized && (Task == null || Task.IsCompleted) && fightsQueue.Count == 0)
        {
            Debug.Log("New Generation");
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
