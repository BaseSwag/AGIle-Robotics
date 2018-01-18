using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AIInput : MonoBehaviour
{

    [System.Serializable]
    public class UnityFloatEvent : UnityEvent<float[]> { };


    [SerializeField]
    public UnityFloatEvent OnInput;


    float[] inputs = new float[2];

    public void SetAIInputs(float[] inputs)
    {
        this.inputs = inputs;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        if (OnInput != null)
            OnInput.Invoke(new float[] { Mathf.Clamp(inputs[0] / 90, -1f, 1f), 1f });

    }
}
