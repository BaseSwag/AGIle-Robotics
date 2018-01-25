using System;
using UnityEditor;
using UnityEngine;

public class FightController : MonoBehaviour {

    public AIInput car1;
    public AIInput car2;

    private Fight fight;

    private GameObject thisGameObject;

    public bool done = false;
    private bool isReset = true;

    public Vector3 StartPosCar1;
    public Vector3 StartRotationCar1;

    public Vector3 StartPosCar2;
    public Vector3 StartRotationCar2;

    public Fight Fight
    {
        get
        {
            return fight;
        }

        set
        {
            Debug.Log($"Fight assigned to {gameObject.name}");
            this.Reset();
            fight = value;
            car1.neuralNetwork = fight.n1;
            car2.neuralNetwork = fight.n2;
        }
    }


    // Use this for initialization
    void Start () {
        thisGameObject = gameObject;
	}
	
	// FixedUpdate is called once per frame
	void FixedUpdate () {
		
	}

    public void OnExitOuterCircle(Collider2D collider)
    {
        if (!done)
        {
            if (collider.gameObject == car1.gameObject)
            {
                fight.tcs.SetResult(new Tuple<double, double>(1, -1));
            }
            else
            {
                fight.tcs.SetResult(new Tuple<double, double>(-1, 1));
            }
            done = true;
        }
    }

    public void Reset()
    {
        car1.transform.localPosition = StartPosCar1;
        car1.transform.localRotation = Quaternion.Euler(StartRotationCar1);

        car2.transform.localPosition = StartPosCar2;
        car2.transform.localRotation = Quaternion.Euler(StartRotationCar2);

        car1.GetComponent<CarController>().dead = false;
        car2.GetComponent<CarController>().dead = false;

        car1.neuralNetwork = null;
        car1.neuralNetwork = null;

        this.fight = null;
        this.isReset = true;
        this.done = false;
    }
}
