using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class CarController : MonoBehaviour {

    float vertical = 0;
    float hortizontal = 0;

    Rigidbody2D rigidbody2D;

	// Use this for initialization
	void Start () {
        rigidbody2D = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {

        rigidbody2D.velocity =  rigidbody2D.velocity * 0.90f;

        if (vertical > 0.1 || vertical < -0.1)
            rigidbody2D.velocity = transform.right * vertical * Time.deltaTime * 750;
        if (hortizontal > 0.1 || hortizontal < -0.1)
            transform.rotation *= Quaternion.Euler(new Vector3(0,0, -hortizontal * Time.deltaTime * 360 / 1.2f));
	}

    public void OnInput(float[] inputs)
    {
        hortizontal = inputs[0];
        vertical = inputs[1];
    }
}
