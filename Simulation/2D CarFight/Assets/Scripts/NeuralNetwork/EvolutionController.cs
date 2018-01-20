using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AGIle_Robotics.Interfaces;
using AGIle_Robotics;
using System;

public class EvolutionController : MonoBehaviour {

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
        generation.Evolve();

        return generation;
    }


    public Task<Tuple<double, double>> fitnessFunction(INeuralNetwork n1, INeuralNetwork n2)
    {
        TaskCompletionSource<Tuple<double, double>> tcs = new TaskCompletionSource<Tuple<double, double>>();
        fights.Add(new Fight(n1, n2, tcs));
        return tcs.Task;

    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
