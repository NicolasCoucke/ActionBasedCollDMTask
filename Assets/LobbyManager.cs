using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviour
{
    public Dropdown dropdown;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartTutorial()
    {
        StaticVariables.TutLevelNumber = 1;
        SceneManager.LoadScene("TutorialScene");
    }

    public void StartSequence()
    {
        StaticVariables.LevelNumber = 1;
        SceneManager.LoadScene("GameScene");
    }

    public void StartPuckSequence()
    {
        StaticVariables.PuckLevelNumber = 65;
        SceneManager.LoadScene("PuckScene");
    }

    public void ChangeNumberPlayers()
    {
        int number = dropdown.value + 2;
        StaticVariables.NumPlayers = number;
     
    }


}
