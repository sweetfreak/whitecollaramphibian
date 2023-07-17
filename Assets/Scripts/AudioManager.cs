using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private AudioSource audioSource;
    private GameSession gameSession;
    private PlayerManager playerManager;

    private bool stillAlive = true;
    private bool musicPlaying;

    [SerializeField] private AudioClip onTimeMusic;
    [SerializeField] private AudioClip lateMusic;
    [SerializeField] private AudioClip almostLateMusic;
    [SerializeField] private AudioClip endMusicBad;
    [SerializeField] private AudioClip endMusicGood;
    [SerializeField] private AudioClip almostLateTransition;
    [SerializeField] private AudioClip lateTransition;
    
    //[SerializeField] private float time = 100f;
    //[SerializeField] private float lateTime = 180f;
    [SerializeField] private bool almostLate = false;
    [SerializeField] private bool onTime = true;
    [SerializeField] private bool notLate = true;
    [SerializeField] private bool gameOver = false;
    private bool endingChosen;
    private bool runningLate = false;
    private bool threeMinutsLeft = false;
    

    // void Awake()
    // {
    //     //this is a singleton pattern
    //     int audioManagers = FindObjectsOfType<GameSession>().Length;
    //
    //     if (audioManagers > 1)
    //     {
    //         Destroy(gameObject);
    //     }
    //     else
    //     {
    //         DontDestroyOnLoad(gameObject);
    //     }
    // }
    void Start()
    {
        gameSession = FindObjectOfType<GameSession>();
        audioSource = FindObjectOfType<AudioSource>();
        playerManager = FindObjectOfType<PlayerManager>();
        StartMusic(onTimeMusic);
    }

    private void Update()
    {
        onTime = gameSession.frogOnTime;
        gameOver = gameSession.gameIsOver;
        runningLate = gameSession.runningLate;
        
        // if (musicPlaying && !gameSession.notMainMenu)
        // {
        //     //audioSource.Stop();
        // }

        if (gameOver && !endingChosen)
        {
            GameOverMusic();
        }

        if (runningLate && !threeMinutsLeft)
        {
            //CAN I GET THIS TO FADE INSTEAD OF STOP??
            ChangeMusic(almostLateTransition, almostLateMusic);
            threeMinutsLeft = true;
        }
        
        if (!onTime && notLate && !gameOver )
        {
            ChangeMusic(lateTransition, lateMusic);
            notLate = false;
        }
    }


    public void StartMusic(AudioClip audioClip)
    {
        Debug.Log("Starting the music");

        musicPlaying = true;
        audioSource.clip = audioClip;
        //Debug.Log("going to play " + audioClip);
        audioSource.Play();
    }
    
    public void StopMusic()
    {
        Debug.Log("run stop music");
        musicPlaying = false;
        audioSource.Stop();
    }

    public void GameOverMusic()
    {
        Debug.Log("run GAMEOVER music");

        AudioClip thisClip;
        audioSource.Stop();
            if (onTime)
            { thisClip = endMusicGood;}
            else
            { thisClip = endMusicBad; }

            endingChosen = true;
            StartMusic(thisClip);
    }

    public void ChangeMusic(AudioClip transition, AudioClip music)
    {
        // while (musicPlaying && audioSource.volume > 0)
        // {
        //     float fadingOutVolume = audioSource.volume;
        //     audioSource.volume = fadingOutVolume - .05f;
        // } 
        // StopMusic();
        // audioSource.clip = transition;
        // audioSource.loop = false;
        StartCoroutine(WaitForTransition(transition, music));
        // audioSource.clip = music;
        // audioSource.loop = true;
        // StartMusic(music);

    }

    IEnumerator WaitForTransition(AudioClip transition, AudioClip music)
    {
     
        StopMusic();
        
        audioSource.PlayOneShot(transition, .6f);
        yield return new WaitForSeconds(3f);
        audioSource.clip = music;
        StartMusic(music);
    }
    
    
}
