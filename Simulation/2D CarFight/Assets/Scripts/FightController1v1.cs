using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AGIle_Robotics.Interfaces;
using AGIle_Robotics;
using System.IO;

public class FightController1v1 : MonoBehaviour
{

    public Trainer trainer;
    public GameObject fightPrefab;
    public FightController controller;
    // Use this for initialization
    void Start()
    {
        if (File.Exists("trainer.json"))
        {
            Time.timeScale = 1;

            trainer = Trainer.Deserialize(File.ReadAllText("trainer.json"));
        
            GameObject arena = (GameObject)Instantiate(fightPrefab, new Vector3(0, 0, 0), new Quaternion(), this.transform);
            controller = arena.GetComponent<FightController>();
            controller.SetResult = false;
            controller.Fight = new Fight(trainer.Best, trainer.Best, null);
            Camera.main.GetComponent<SmartCamera>().targets = new Transform[] { arena.transform };
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(controller)
            if(controller.done)
            {
                controller.Fight = new Fight(trainer.Best, trainer.Best, null);
            }
    }
}
