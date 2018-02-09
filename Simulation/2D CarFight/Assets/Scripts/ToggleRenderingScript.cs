using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleRenderingScript : MonoBehaviour {

    public Camera mainCamera;
    private void Start()
    {
        mainCamera = Camera.main;
    }
    public void OnToggleRendering()
    {
        mainCamera.enabled = !mainCamera.enabled;
    }
}
