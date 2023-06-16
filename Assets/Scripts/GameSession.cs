using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    [Header("Timer")]
 
    [SerializeField] private float timeAtStart;
    [SerializeField] private float timeLeft;
    [SerializeField] private bool frogOnTime = true;
    public bool notMainMenu = true;
    
    [Header("Fruits Eaten")]
    [SerializeField] private int fruitsEaten = 0;
    
    [Header("Death/Spawn/Checkpoints")]
    [SerializeField] private float respawnDelay = 2f;
    // [SerializeField] private bool checkpointReached = false;
    // [SerializeField] GameObject spawnPoint;
    // [SerializeField] GameObject checkpoint;
    // public Vector3 currentSpawnPoint = new Vector3();

    [Header("Pause Stuff")] 
    [SerializeField] private bool needPause = true;
    public bool pausePressed;
    public static bool gameIsPaused;
    [SerializeField] private GameObject pauseMenuGO;
    

    [Header("Canvas")]
    [SerializeField] private TextMeshProUGUI timeText;
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
        //playerLives = playerLivesAtStart;
        timeLeft = timeAtStart;
        fruitsEatenText.text = "Fruits: " + fruitsEaten;
        // checkpointReached = false;
        // currentSpawnPoint = spawnPoint.transform.position;
    }

    private void Update()
    {
        TimerFunction();
        PausingGame();
    }

    void PausingGame()
    {
        if (needPause && pausePressed)
        {
            needPause = false;
            if (gameIsPaused)
            {
                ResumeGame();
                
            }
            else 
            {
                PauseGame();

            }
        }
    }

    void PauseGame()
    {
        pauseMenuGO.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
    }

    public void ResumeGame()
    {
        pauseMenuGO.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
        needPause = true;
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

    // public void CheckPointProcess()
    // {
    //     currentSpawnPoint = checkpoint.transform.position;
    //     
    //
    // }

    void TimerFunction()
    {
        if (notMainMenu)
        {
            if (timeLeft > 0 && frogOnTime)
            {
                timeText.color = Color.white;
                timeLeft -= Time.deltaTime;
            }
            else
            {
                frogOnTime = false;
                timeLeft += Time.deltaTime;
                timeText.color = Color.red;
            }
        }

        DisplayTime(timeLeft);

    }
    
    void DisplayTime(float timeToDisplay)
    {
        float minutes = Mathf.FloorToInt(timeToDisplay / 60);  
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);
        if (frogOnTime)
        {
            timeText.text = ("Timer: " + string.Format("{0:00}:{1:00}", minutes, seconds) + " early");
        }
        else
        {
            timeText.text = ("Timer: " + string.Format("{0:00}:{1:00}", minutes, seconds) + " late");

        }
    }
    

    public void ResetGameSession()
    {
        ResumeGame();
        notMainMenu = true;
        //may need to change the scene number eventually
        fruitsEaten = 0;
        timeLeft = timeAtStart;
        timeText.color = Color.white;
        //playerLives = playerLivesAtStart;
        // checkpointReached = false;
        SceneManager.LoadSceneAsync("TitleScreen");
        Destroy(gameObject);
        
    }

    void ResetScene()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadSceneAsync(currentSceneIndex);
    }

    IEnumerator DeathDelay()
    {
        yield return new WaitForSecondsRealtime(respawnDelay);
        ResetScene();

    }
}
