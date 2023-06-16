using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private ScenePersist scenePersist;
    private GameSession gameSession;
    
    void Start()
    {
       scenePersist = FindObjectOfType<ScenePersist>();
       gameSession = FindObjectOfType<GameSession>();
       if (gameSession)
       {
           Destroy(scenePersist);
       }
       
       

    }
    private void Update()
    {
        if (Input.anyKey)
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        if (gameSession != null)
        {
                    gameSession.notMainMenu = true;
                    Destroy(gameSession.gameObject);
        }
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadSceneAsync(currentSceneIndex + 1);
    }
}
