using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadLevelScriot : MonoBehaviour {
    public int levelIndex = 0;
	// Use this for initialization
	public void OnLoadLevelClick()
    {
        SceneManager.LoadScene(levelIndex);
    }
}
