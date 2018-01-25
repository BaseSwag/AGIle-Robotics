using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HumanInput : MonoBehaviour {

    [System.Serializable]
    public class UnityFloatEvent : UnityEvent<float[]> { };


    [SerializeField]
    public UnityFloatEvent OnInput;

	// Use this for initialization
	void Start () {
		
	}
	
	// FixedUpdate is called once per frame
	void FixedUpdate () {
        OnInput.Invoke(new float[] { Input.GetAxis("Horizontal"), Input.GetAxis("Vertical") });
	}
}
