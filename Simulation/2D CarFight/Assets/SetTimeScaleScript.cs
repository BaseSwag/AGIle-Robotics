using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTimeScaleScript : MonoBehaviour {
    public float TimeScale = 1;

    public void SetTimeScale()
    {
        Debug.Log("SetTimeSpeed to " + TimeScale);
        Time.timeScale = TimeScale;
    }
}
