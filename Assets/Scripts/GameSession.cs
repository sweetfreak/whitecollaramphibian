using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSession : MonoBehaviour
{
    [Header("Timer")]
 
    public float timeAtStart;
    public float timeLeft;
    public bool frogOnTime = true;
    public bool notMainMenu = true;
    public bool runningLate = false;
    public float runningLateSeconds = 180f;
    
    [Header("Fruits Eaten")]
    [SerializeField] private int fruitsEaten = 0;
    
    [Header("Death/Spawn/Checkpoints")]
    [SerializeField] private float respawnDelay = 2f;

    [SerializeField] private GameObject playerGameObject;
    

    [SerializeField] private CinemachineStateDrivenCamera cmCamera;
    // [SerializeField] private bool checkpointReached = false;
    // [SerializeField] GameObject spawnPoint;
    // [SerializeField] GameObject checkpoint;
    // public Vector3 currentSpawnPoint = new Vector3();

    [Header("Pause Stuff")] 
    public bool inGame = true;
    [SerializeField] private bool needPause = true;
    public bool pausePressed;
    public static bool gameIsPaused;
    [SerializeField] private GameObject pauseMenuGO;
    

    [Header("Canvas")]
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI fruitsEatenText;
    public GameObject endingCanvas;
    public GameObject loseText;
    public GameObject winText;
    public GameObject finalScoreGO;
    public bool gameIsOver = false;
    
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
        cmCamera = FindObjectOfType<CinemachineStateDrivenCamera>();
        // checkpointReached = false;
        // currentSpawnPoint = spawnPoint.transform.position;
    }

    private void Update()
    {
        TimerFunction();
        PausingGame();

        // if (!cmCamera.m_AnimatedTarget)
        // {
        //     cmCamera.m_AnimatedTarget = FindObjectOfType<PlayerManager>().GetComponent<Animator>();
        // };
    }

    void PausingGame()
    {
        if (needPause && pausePressed && inGame)
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
        AudioSource audiosource = FindObjectOfType<AudioSource>();
        audiosource.Pause();
    }

    public void ResumeGame()
    {
        pauseMenuGO.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
        needPause = true;
        AudioSource audiosource = FindObjectOfType<AudioSource>();
        audiosource.UnPause();
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
        if (notMainMenu && inGame)
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
        // else
        // {
        //     finalScoreGO.SetActive(true);
        // }

        if (timeLeft < runningLateSeconds)
        {
            runningLate = true;
        }
        DisplayTime(timeLeft);
        

        //DisplayTime(timeLeft);

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
        //gameEnded = true;
        //may need to change the scene number eventually
        fruitsEaten = 0;
        timeLeft = timeAtStart;
        timeText.color = Color.white;
        //playerLives = playerLivesAtStart;
        // checkpointReached = false;
        ScenePersist scenePersist = FindObjectOfType<ScenePersist>();
        scenePersist.ResetScenePersist();
        
        SceneManager.LoadSceneAsync("TitleScreen");
        Destroy(gameObject);
        
    }

    // void ResetScene()
    // {
    //     int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
    //     SceneManager.LoadSceneAsync(currentSceneIndex);
    // }

    IEnumerator DeathDelay()
    {
        yield return new WaitForSecondsRealtime(respawnDelay);
        Instantiate(playerGameObject);
        //Debug.Log("delay over");
        //cinemachine follow playergameobject
        //cmCamera.m_AnimatedTarget = FindObjectOfType<PlayerManager>().GetComponent<Animator>();

        //ResetScene();

    }
}
