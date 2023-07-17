using System;
using System.Collections;
using System.Collections.Generic;
//using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelExit : MonoBehaviour
{
    [SerializeField] private float poseDelay = 3f;
    //[SerializeField] private float walkOffDelay = 2f;
    //[SerializeField] private float levelLoadDelay = 2f;
    [SerializeField] private AudioClip levelCompleteTheme;
    [SerializeField] private ParticleSystem confetti;
    //[SerializeField] private Image blackSquareSprite;
    [SerializeField] private Animator blackFadeAnimator;
    private GameObject gameManager;
    private GameSession gameSession;

    private void Update()
    {
        if (!gameSession || !blackFadeAnimator)
        {
            gameManager = GameObject.Find("GameSessionManager");
            gameSession = gameManager.GetComponent<GameSession>();
            blackFadeAnimator = gameManager.GetComponent<Animator>();
            blackFadeAnimator.SetBool("FadeOut", false);
            
        }
        // blackFadeAnimator = GameObject.Find("GameSessionManager").GetComponent<Animator>();
        // blackFadeAnimator.SetBool("FadeOut", false);
        // gameSession = FindObjectOfType<GameSession>();
        
        
        

    }
    
    //update with finding gameSession??

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            gameSession.inGame = false;
            PlayerInput input = FindObjectOfType<PlayerInput>();
            input.currentActionMap.Disable();
            col.attachedRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            col.transform.localScale = new Vector3(1, 1, 1);
            col.attachedRigidbody.velocity = new Vector2(0,0);
            confetti.Play();
            // col.attachedRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            // col.attachedRigidbody.constraints = RigidbodyConstraints2D.None;
            
            //GetComponent<AudioManager>().StopMusic();
            AudioSource.PlayClipAtPoint(levelCompleteTheme, Camera.main.transform.position,.5f);
            StartCoroutine((LevelFinishCoroutine(col)));
            
        }
    }

    IEnumerator LevelFinishCoroutine(Collider2D col)
    {
        col.transform.position   = new Vector3(confetti.transform.position.x + .3f, confetti.transform.position.y - .35f);
        col.transform.localScale = new Vector3(1f, 1f, 1f);
        col.GetComponent<Animator>().SetBool("EndPose", true);
        col.GetComponent<Animator>().SetBool("LevelOver", true);

        
        yield return new WaitForSeconds(poseDelay);
        col.GetComponent<Animator>().SetBool("EndPose", false);
        col.GetComponent<Animator>().SetBool("isRunning", true);
        col.GetComponent<Animator>().SetBool("isJumping", false); 
        // yield return new WaitForSeconds(walkOffDelay);
        col.attachedRigidbody.constraints = RigidbodyConstraints2D.None;
        col.attachedRigidbody.velocity = new Vector2((.25f * Time.deltaTime), 0);
        StartCoroutine(FadeOut());
        

        
    }

    public void LoadNextLevel()
    {
        gameSession.inGame = true;
        //FOR REGULAR GAME 
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        {
            gameSession.notMainMenu = false;
            
            nextSceneIndex = 0;
        }

        //need to reset scene persists before a new level
        //Does not reset whole scene persist anymore
        FindObjectOfType<ScenePersist>().ResetScenePersist();
        
        SceneManager.LoadSceneAsync(nextSceneIndex);
    }

    IEnumerator FadeOut()
    {
        blackFadeAnimator.SetBool("FadeOut", true);
        yield return new WaitForSeconds(2.5f);
        LoadNextLevel();
                
        
       
    }
}
