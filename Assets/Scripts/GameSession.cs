using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    [SerializeField] private int playerLivesAtStart= 10;
    [SerializeField] private int playerLives;
    [SerializeField] private int fruitsEaten = 0;
    [SerializeField] private float respawnDelay = 2f;

    [SerializeField] private TextMeshProUGUI livesText;
    [SerializeField] private TextMeshProUGUI fruitsEatenText;
    
    void Awake()
    {
        //this is a singleton pattern
        int numGameSessions = FindObjectsOfType<GameSession>().Length;

        if (numGameSessions > 1)
        {
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    private void Start()
    {
        playerLives = playerLivesAtStart;
        livesText.text = "Lives: " + playerLives;
        fruitsEatenText.text = "Fruits: " + fruitsEaten;
        // livesText.text = playerLives.ToString();
        // fruitsEatenText.text = fruitsEaten.ToString();
        
    }


    public void ProcessPlayerDeath()
    {
        StartCoroutine(DeathDelay());
    }

    public void AddFruits()
    {
        fruitsEaten++;
        fruitsEatenText.text = "Fruits: " + fruitsEaten;
    }

    public void ResetGameSession()
    {
        //may need to change the scene number eventually
        fruitsEaten = 0;
        playerLives = playerLivesAtStart;
        SceneManager.LoadScene("TitleScreen");
        Destroy(gameObject);
    }

    void TakeLife()
    {
        Debug.Log("play had this many lives: " + playerLives);
        playerLives--;
        Debug.Log("player now has " + playerLives);
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        livesText.text = "Lives: " + playerLives;
        SceneManager.LoadScene(currentSceneIndex);
        
    }

    IEnumerator DeathDelay()
    {
        yield return new WaitForSecondsRealtime(respawnDelay);
        
        if (playerLives > 1)
        {
            TakeLife();
        }
        else
        {
            FindObjectOfType<ScenePersist>().ResetScenePersist();
            ResetGameSession();
            
        }
        
    }
}
