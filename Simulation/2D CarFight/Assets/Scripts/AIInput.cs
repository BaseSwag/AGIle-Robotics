using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Threading.Tasks;

public class AIInput : MonoBehaviour
{

    [System.Serializable]
    public class UnityFloatEvent : UnityEvent<float[]> { };


    [SerializeField]
    public UnityFloatEvent OnInput;


    float[] inputs = new float[6];

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
        float hor = Mathf.Clamp(inputs[0] / 90, -1f, 1f);
        float vert = 1;

        if (inputs[4] > 0.1f)
        {
            vert = -1;
            hor = -0.2f;
        }

        if (inputs[5] > 0.1f)
        {
            vert = 1;
            hor = 0.2f;
        }

        if (OnInput != null)
            OnInput.Invoke(new float[] { hor, vert });

    }
}
