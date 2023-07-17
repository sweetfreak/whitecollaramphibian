using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.Controls;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private ScenePersist scenePersist;
    private GameSession gameSession;
    private bool creditsOn = false;

    [SerializeField] private AudioSource myAudioSource;
   // private Animator animator;

    private Canvas canvas;
    [SerializeField] private GameObject logo;
    [SerializeField] private GameObject creditsGO;
    [SerializeField] private GameObject playButton;
    

    void Start()
    {
       scenePersist = FindObjectOfType<ScenePersist>();
       gameSession = FindObjectOfType<GameSession>();
     
       if (gameSession)
       {
           myAudioSource.Stop();
           Destroy(scenePersist);
           
       }

    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.Return))
        {
            if (creditsOn)
            {
                StartCredits();
            }
            else
            { StartGame();

            }
        }

        //StartCoroutine(JumpSqueeze(0.8f, 1.1f, 2f));

    }

    // IEnumerator JumpSqueeze(float xSqueeze, float ySqueeze, float seconds) {
    //     Vector3 originalSize = transform.localScale;
    //     Vector3 newSize = new Vector3(xSqueeze * Mathf.Sign(transform.localScale.x), ySqueeze, originalSize.z);
    //     float t = 0f;
    //     while (t <= 3.0) {
    //         t += Time.deltaTime / seconds;
    //         logo.transform.localScale = Vector3.Lerp(originalSize, newSize, t);
    //         yield return null;
    //     }
    //     t = 0f;
    //     while (t <= 3.0) {
    //         t += Time.deltaTime / seconds;
    //         logo.transform.localScale = Vector3.Lerp(newSize, originalSize, t);
    //         yield return null;
    //     }
    // }
    
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

    public void StartCredits()
    {
        creditsOn = !creditsOn;
        if (creditsOn)
        {
            logo.SetActive(true);
            playButton.SetActive(true);
            creditsGO.SetActive(false);      
        }
        else
        {
            logo.SetActive(false);
            playButton.SetActive(false);
            creditsGO.SetActive(true);    
        }
        //turn off logo
        //turn off play button
        //turn on credits object
    }
}
