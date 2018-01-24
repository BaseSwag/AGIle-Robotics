using System;
using UnityEditor;
using UnityEngine;

public class FightController : MonoBehaviour {

    public AIInput car1;
    public AIInput car2;

    private Fight fight;

    private GameObject thisGameObject;

    private bool done = false;
    private bool isReset = true;
    
    public Fight Fight
    {
        get
        {
            return fight;
        }

        set
        {
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
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnExitOuterCircle(Collider2D collider)
    {
        if (!done)
        {
            done = true;
            if (collider.gameObject == car1.gameObject)
            {
                fight.tcs.SetResult(new Tuple<double, double>(1, -1));
            }
            else
            {
                fight.tcs.SetResult(new Tuple<double, double>(-1, 1));
            }
        }
    }

    public void Reset()
    {
        PrefabUtility.ResetToPrefabState(thisGameObject);
    }
}
