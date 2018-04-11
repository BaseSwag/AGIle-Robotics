using System;
using UnityEngine;

public class FightController : MonoBehaviour
{

    public AIInput car1;
    public AIInput car2;

    private Fight fight;

    private GameObject thisGameObject;

    public bool done = false;
    private bool isReset = true;

    public int fightTimeoutSeconds = 30;
    public float tickTimeoutLeft = 0;

    public Transform circleTransform;

    public Vector3 StartPosCar1;
    public Vector3 StartRotationCar1;

    public Vector3 StartPosCar2;
    public Vector3 StartRotationCar2;

    public bool SetResult = true;
    public bool RealSumo = false;

    public Fight Fight
    {
        get
        {
            return fight;
        }

        set
        {
            //Debug.Log($"Fight assigned to {gameObject.name}");
            this.Reset();
            fight = value;
            car1.transform.Rotate(new Vector3(0, 0, 1), UnityEngine.Random.Range(0, 360f));
            car2.transform.Rotate(new Vector3(0, 0, 1), UnityEngine.Random.Range(0, 360f));
            car1.neuralNetwork = fight.n1;
            car2.neuralNetwork = fight.n2;
        }
    }

    // Use this for initialization
    void Start()
    {
        tickTimeoutLeft = fightTimeoutSeconds;
        thisGameObject = gameObject;
    }

    void OnCarLost(bool car1)
    {
        if (!done)
        {
            Debug.Log("Fight over");
            done = true;
            if (SetResult)
            {
                EvolutionController.fightsDone++;
                if (car1)
                {
                    Fight.tcs?.SetResult(new Tuple<double, double>(-1, 1));
                }
                else
                {
                    Fight.tcs?.SetResult(new Tuple<double, double>(1, -1));
                }
            }
        }
    }

    void OnCarLost(double fitness)
    {
        if (!done)
        {
            Debug.Log("Fight over");
            done = true;
            if (SetResult)
            {
                EvolutionController.fightsDone++;
                Fight.tcs?.SetResult(new Tuple<double, double>(fitness, fitness));
            }
        }
    }

    // FixedUpdate is called once per frame
    void FixedUpdate()
    {
        if (!done)
        {
            tickTimeoutLeft -= Time.fixedDeltaTime;
            if (tickTimeoutLeft <= 0)
            {
                if (Fight != null)
                    if (RealSumo)
                    {
                        OnCarLost(Vector3.Distance(car1.transform.position, this.transform.position) > Vector3.Distance(car2.transform.position, this.transform.position));
                    }
                    else
                    {
                        OnCarLost(-0.5);
                    }

                tickTimeoutLeft = fightTimeoutSeconds;
            }
        }

    }

    public void OnExitOuterCircle(Collider2D collider)
    {
        if (collider.gameObject == car1.gameObject || collider.gameObject == car2.gameObject)
            OnCarLost(collider.gameObject == car1.gameObject);
    }

    public void Reset()
    {
        car1.transform.localPosition = StartPosCar1;
        car1.transform.localRotation = Quaternion.Euler(StartRotationCar1);
        car1.GetComponent<CarController>().dead = false;
        car1.GetComponent<Rigidbody2D>().velocity = new Vector3();
        car1.GetComponent<Rigidbody2D>().angularVelocity = 0;
        car1.neuralNetwork = null;

        car2.transform.localPosition = StartPosCar2;
        car2.transform.localRotation = Quaternion.Euler(StartRotationCar2);
        car2.GetComponent<CarController>().dead = false;
        car2.GetComponent<Rigidbody2D>().velocity = new Vector3();
        car2.GetComponent<Rigidbody2D>().angularVelocity = 0;
        car2.neuralNetwork = null;

        tickTimeoutLeft = fightTimeoutSeconds;
        this.fight = null;
        this.isReset = true;
        this.done = false;
    }
}
