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
    public float sensorForward = 0;
    public bool inTriggerBackward = true;
    public float sensorBackward = 0;

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

            if (vertical > 0.1 || vertical < -0.1)
            {
                rigidbody2D.drag = 2;
                rigidbody2D.AddForce(transform.right * vertical * Time.fixedDeltaTime * speedMultiplier, ForceMode2D.Force);
            }
            else
            {
                rigidbody2D.drag = 6;
            }


            if (hortizontal > 0.1 || hortizontal < -0.1)
                rigidbody2D.angularVelocity = -hortizontal * 360 / 1.2f;
            //transform.rotation *= Quaternion.Euler(new Vector3(0, 0, -hortizontal * Time.fixedDeltaTime * 360 / 1.2f));

            Vector3 difference = target.position - transform.position;
            float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

            lookRotationRelative = rotationZ - this.transform.rotation.eulerAngles.z;

            while (lookRotationRelative > 180)
                lookRotationRelative -= 360;

            while (lookRotationRelative < -180)
                lookRotationRelative += 360;

            if (inTriggerBackward)
                if (sensorBackward > 0)
                    sensorBackward -= Time.fixedDeltaTime;
                else
                    sensorBackward = 0;

            if (inTriggerForward)
                if (sensorForward > 0)
                    sensorForward -= Time.fixedDeltaTime;
                else
                    sensorForward = 0;

            OnSetAIInputs.Invoke(new float[] { -lookRotationRelative, 0, hortizontal, vertical, sensorForward, sensorBackward });

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
            sensorForward = 1;
            inTriggerForward = false;
        }

        if (collider.gameObject.transform == this.LightSensorBackward)
        {
            sensorBackward = 1;
            inTriggerBackward = false;
        }
    }

    public void OnEnterInnerCircle(Collider2D collider)
    {
        if (collider.gameObject.transform == this.LightSensorForward)
        {
            sensorForward = 1;
            inTriggerForward = true;
        }

        if (collider.gameObject.transform == this.LightSensorBackward)
        {
            sensorBackward = 1;
            inTriggerForward = true;
        }
    }

    public void OnExitOuterCircle(Collider2D collider)
    {
        if (collider.gameObject == this.gameObject)
        {
            Debug.Log("Lost! " + gameObject.name);
            dead = true;
        }
    }

    public void OnInput(float[] inputs)
    {
        hortizontal = inputs[0];
        vertical = inputs[1];
    }
}
