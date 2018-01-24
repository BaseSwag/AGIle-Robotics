using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using AGIle_Robotics.Interfaces;
using AGIle_Robotics;
using SuperTuple;
using System;



public class EvolutionController : MonoBehaviour
{

    public GameObject fightPrefab;
    public FightController[] fights = new FightController[5];
    public Queue<Fight> fightsQueue = new Queue<Fight>();

    public Task Task;
    public bool initialized = false;
    public volatile bool needsRechecking = false;

    public Trainer trainer;
    Generation newGeneration()
    {

        var generation = new Generation(
                size: 50,
                popSize: new Tuple<int, int>(25, 50),
                ports: new Tuple<int, int>(6, 3),
                length: new Tuple<int, int>(5, 25),
                width: new Tuple<int, int>(5, 20),
                weightRange: new Tuple<double, double>(-2, 2),
                activateWith: Math.Tanh);

        generation.Create().Wait();
        generation.Evaluate(fitnessFunction);

        return generation;
    }


    public void CheckForNewFight()
    {
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
        fight.tcs.Task.ContinueWith(t => {
            Debug.Log("Continuing");
            needsRechecking = true;
        });
        needsRechecking = true;
        return tcs.Task;

    }
    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < fights.Length; i++)
        {
            GameObject arena = (GameObject)Instantiate(fightPrefab, new Vector3(4000 * i, 0, 0), new Quaternion(), this.transform);
            FightController controller = arena.GetComponent<FightController>();
            fights[i] = controller;
        }

        
        Debug.Log("Arenas created");

        trainer = new Trainer(
            transitionRatio: 0.5,
            randomRatio: 0.1,
            mutationRatio: 0.1
            );

        trainer.FitnessFunction = fitnessFunction;
        
        Debug.Log("Trainer object created");

        Task = Task.Run(async () => {
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

    // Update is called once per frame
    void Update()
    {
        if(initialized && (Task == null || Task.IsCompleted))
        {
            Debug.Log($"Generation {trainer.Level}");
            Task = trainer.EvaluateAndEvolve();
        }

        if (needsRechecking)
        {
            CheckForNewFight();
        }
    }
}
