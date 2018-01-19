using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnExitListener : MonoBehaviour {

    [System.Serializable]
    public class UnityOnTriggerEvitEvent : UnityEvent<Collider2D> { };


    [SerializeField]
    public UnityOnTriggerEvitEvent OnExit;

    [SerializeField]
    public UnityOnTriggerEvitEvent OnEnter;

    private void OnTriggerExit2D(Collider2D collision)
    {
        OnExit.Invoke(collision);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnEnter.Invoke(collision);
    }
}
