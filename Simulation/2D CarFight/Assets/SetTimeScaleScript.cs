using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTimeScaleScript : MonoBehaviour {
    public float TimeScale = 0;
    public void SetTimeScale()
    {
        Time.timeScale = TimeScale;
    }
}
