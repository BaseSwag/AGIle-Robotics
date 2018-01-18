using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OnExitListener : MonoBehaviour {

    [System.Serializable]
    public class UnityOnTriggerEvitEvent : UnityEvent<Collider2D> { };


    [SerializeField]
    public UnityOnTriggerEvitEvent OnExit;

    private void OnTriggerExit2D(Collider2D collision)
    {
        OnExit.Invoke(collision);
    }
}
