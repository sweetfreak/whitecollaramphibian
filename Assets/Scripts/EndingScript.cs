using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class EndingScript : MonoBehaviour
{
    private bool onTime = true;
    [SerializeField] private GameSession gameSession;
    [SerializeField] private Animator endingsAnimator;
    private LevelExit levelExit;
    public bool gameIsOver;
    
    //void Start()
    void Start()
    {
        gameSession = FindObjectOfType<GameSession>();
        //levelExit = GetComponent<LevelExit>();
        //endingsAnimator = FindObjectOfType<Animator>();
        //onTime = gameSession.frogOnTime;
        

        

    }

    private void Update()
    {
        if (!gameSession)
        {
            gameSession = FindObjectOfType<GameSession>();
            
        }
        else
        {
            gameSession.inGame = false;
            gameSession.endingCanvas.SetActive(true);
            gameSession.finalScoreGO.SetActive(true);
            gameSession.gameIsOver = true;
            StartCoroutine(EndingRoutine());
        }
       
    }

    IEnumerator EndingRoutine()
    {
        onTime = gameSession.frogOnTime;
        
        if (onTime)
        {
            gameSession.endingCanvas.transform.GetChild(0).gameObject.SetActive(true);
            //gameSession.winText.SetActive(true);
            Debug.Log("on time!");
            endingsAnimator.SetBool("Won", true);
            endingsAnimator.SetBool("Lost", false);
        }
        else
        {

            gameSession.endingCanvas.transform.GetChild(1).gameObject.SetActive(true);
            Debug.Log("not on time!");
            //gameSession.loseText.SetActive(true);

            endingsAnimator.SetBool("Lost", true);
            endingsAnimator.SetBool("Won", false);

        }
       yield return new WaitForSeconds(5);
       gameSession.endingCanvas.SetActive(false);
       gameSession.winText.SetActive(false);
       gameSession.loseText.SetActive(false);
       gameSession.notMainMenu = false;

       //need to reset scene persists before a new level
    //FindObjectOfType<ScenePersist>().ResetScenePersist();
    SceneManager.LoadSceneAsync(0);
    }
    
}
