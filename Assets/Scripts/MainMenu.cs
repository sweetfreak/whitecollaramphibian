using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Update()
    {
        if (Input.anyKey)
        {
            StartGame();
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Level 1");
    }
}
