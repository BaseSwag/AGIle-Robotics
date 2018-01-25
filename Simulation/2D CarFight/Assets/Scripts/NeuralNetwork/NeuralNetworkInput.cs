using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AGIle_Robotics.Interfaces;
using AGIle_Robotics;
using System.Threading.Tasks;
using System;

public class NeuralNetworkInput : MonoBehaviour
{

   
    public List<Fight> fights = new List<Fight>();


    // Use this for initialization
    void Start()
    {
        // fightBerechnen();
        fights[0].tcs.SetResult(new Tuple<double,double>(0,0));
    }

    // FixedUpdate is called once per frame
    void FixedUpdate()
    {

    }

    
}
