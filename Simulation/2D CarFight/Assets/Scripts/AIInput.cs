using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using System.Threading.Tasks;
using AGIle_Robotics.Interfaces;
using System;

public class AIInput : MonoBehaviour
{

    [System.Serializable]
    public class UnityFloatEvent : UnityEvent<float[]> { };


    [SerializeField]
    public UnityFloatEvent OnInput;



    public INeuralNetwork neuralNetwork;

    public int delay = 10;

    private int currentdelay = 10;

    float[] inputs = new float[6];

    public void SetAIInputs(float[] inputs)
    {
        this.inputs = inputs;
    }

    // Use this for initialization
    void Start()
    {

    }

    // FixedUpdate is called once per frame
    void FixedUpdate()
    {
        if (currentdelay++ % delay == 0)
        {
            if (neuralNetwork != null)
            {
                double[] outputs = neuralNetwork.Activate(Array.ConvertAll(inputs, x => (double)x));
                OnInput?.Invoke(Array.ConvertAll(outputs, x => (float)x));
            }
        }
    }
}
