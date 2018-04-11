using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class CarController : MonoBehaviour
{

    [System.Serializable]
    public class UnityFloatEvent : UnityEvent<float[]> { };


    [SerializeField]
    public UnityFloatEvent OnSetAIInputs;


    float vertical = 0;
    float hortizontal = 0;

    public float lookRotationRelative = 0;
    public bool inTriggerForward = true;
    public bool inTriggerBackward = true;

    public float speedMultiplier = 0;

    public float currentSpeed = 0;

    public Transform LightSensorForward;
    public Transform LightSensorBackward;

    public Transform target;

    private Vector3 lastPos;

    Rigidbody2D rigidbody2D;

    float count = 0;

    public bool dead = false;

    // Use this for initialization
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        lastPos = transform.position;
    }

    // FixedUpdate is called once per frame
    void FixedUpdate()
    {
        if (!dead)
        {
            //rigidbody2D.velocity = rigidbody2D.velocity * 0.5f;

            if (vertical > 0.7 || vertical < -0.7)
            {
                rigidbody2D.drag = 2;
                rigidbody2D.AddForce(transform.right * vertical * Time.fixedDeltaTime * speedMultiplier, ForceMode2D.Force);
            }
            else
            {
                rigidbody2D.drag = 6;
            }


            if (hortizontal > 0.5 || hortizontal < -0.5)
                rigidbody2D.angularVelocity = -hortizontal * 360 / 1.2f;
            //transform.rotation *= Quaternion.Euler(new Vector3(0, 0, -hortizontal * Time.fixedDeltaTime * 360 / 1.2f));

            Vector3 difference = target.position - transform.position;
            float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

            lookRotationRelative = rotationZ - this.transform.rotation.eulerAngles.z;

            while (lookRotationRelative > 180)
                lookRotationRelative -= 360;

            while (lookRotationRelative < -180)
                lookRotationRelative += 360;

            
            float newLookRotation;
            if(lookRotationRelative <= -45 / 2 || lookRotationRelative >= 45 / 2)
            {
                newLookRotation = -1;
            }
            else
            {
                newLookRotation = lookRotationRelative / 45 + 0.5f;
            }


            //OnSetAIInputs.Invoke(new float[] { newLookRotation, 0, hortizontal, vertical, sensorForward, sensorBackward });
            OnSetAIInputs.Invoke(new float[] { newLookRotation, 0, 0, 0, inTriggerForward ? 1 : 0, inTriggerBackward ? 1 : 0});

            count += Time.fixedDeltaTime;

            if (count > 2)
            {
                currentSpeed = Vector3.Distance(lastPos, transform.position) / count;

                lastPos = transform.position;
                count = 0;
            }
        }

    }

    public void OnExitInnerCircle(Collider2D collider)
    {
        if (collider.gameObject.transform == this.LightSensorForward)
        {
            inTriggerForward = false;
        }

        if (collider.gameObject.transform == this.LightSensorBackward)
        {
            inTriggerBackward = false;
        }
    }

    public void OnEnterInnerCircle(Collider2D collider)
    {
        if (collider.gameObject.transform == this.LightSensorForward)
        {
            inTriggerForward = true;
        }

        if (collider.gameObject.transform == this.LightSensorBackward)
        {
            inTriggerForward = true;
        }
    }

    public void OnExitOuterCircle(Collider2D collider)
    {
        if (collider.gameObject == this.gameObject)
        {
            dead = true;
        }
    }

    public void OnInput(float[] inputs)
    {
        hortizontal = inputs[0];
        vertical = inputs[1];
    }
}
