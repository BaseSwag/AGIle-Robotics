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

    public Transform LightSensorForward;
    public Transform LightSensorBackward;

    public Transform target;

    Rigidbody2D rigidbody2D;

    // Use this for initialization
    void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

        rigidbody2D.velocity = rigidbody2D.velocity * 0.93f;

        if (vertical > 0.1 || vertical < -0.1)
            rigidbody2D.AddForce(transform.right * vertical * Time.deltaTime * 100000, ForceMode2D.Force);
        if (hortizontal > 0.1 || hortizontal < -0.1)
            transform.rotation *= Quaternion.Euler(new Vector3(0, 0, -hortizontal * Time.deltaTime * 360 / 1.2f));

        Vector3 difference = target.position - transform.position;
        float rotationZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;

        lookRotationRelative = rotationZ - this.transform.rotation.eulerAngles.z;

        while (lookRotationRelative > 180)
            lookRotationRelative -= 360;

        while (lookRotationRelative < -180)
            lookRotationRelative += 360;

        if (inTriggerBackward)
            if (sensorBackward > 0)
                sensorBackward -= Time.deltaTime;
            else
                sensorBackward = 0;

        if (inTriggerForward)
            if (sensorForward > 0)
                sensorForward -= Time.deltaTime;
            else
                sensorForward = 0;

        OnSetAIInputs.Invoke(new float[] { -lookRotationRelative, 0, hortizontal, vertical, sensorForward, sensorBackward });

    }

    public void OnExitInnerCircle(Collider2D collider)
    {
        if (collider.gameObject.transform == this.LightSensorForward)
        {
            sensorForward = 1;
            inTriggerForward = false;
            Debug.Log("Trigger LightSensor Forward " + gameObject.name);
        }

        if (collider.gameObject.transform == this.LightSensorBackward)
        {
            sensorBackward = 1;
            inTriggerBackward = false;
            Debug.Log("Trigger LightSensor Backward " + gameObject.name);
        }
    }

    public void OnEnterInnerCircle(Collider2D collider)
    {
        if (collider.gameObject.transform == this.LightSensorForward)
        {
            sensorForward = 1;
            inTriggerForward = true;
            Debug.Log("Enter Trigger LightSensor Forward " + gameObject.name);
        }

        if (collider.gameObject.transform == this.LightSensorBackward)
        {
            sensorBackward = 1;
            inTriggerForward = true;
            Debug.Log("Enter Trigger LightSensor Backward " + gameObject.name);
        }
    }

    public void OnExitOuterCircle(Collider2D collider)
    {
        if (collider.gameObject == this.gameObject)
            Debug.Log("Lost! " + gameObject.name);
    }

    public void OnInput(float[] inputs)
    {
        hortizontal = inputs[0];
        vertical = inputs[1];
    }
}
