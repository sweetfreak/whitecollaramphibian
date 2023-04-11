using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExit : MonoBehaviour
{
    [SerializeField] private float levelLoadDelay = 6f;
    [SerializeField] private AudioClip levelCompleteTheme;
    [SerializeField] private ParticleSystem confetti;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag == "Player")
        {
            col.attachedRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
            confetti.Play();
            //GetComponent<AudioManager>().StopMusic();
            AudioSource.PlayClipAtPoint(levelCompleteTheme, Camera.main.transform.position,.5f);
            StartCoroutine((LevelFinishCoroutine()));
        }
    }

    IEnumerator LevelFinishCoroutine()
    {
        yield return new WaitForSecondsRealtime(levelLoadDelay);
       
        
        //FOR REGULAR GAME 
        // int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        // int nextSceneIndex = currentSceneIndex + 1;
        // if (nextSceneIndex == SceneManager.sceneCountInBuildSettings)
        // {
        //     nextSceneIndex = 0;
        // }
        // //need to reset scene persists before a new level
        // FindObjectOfType<ScenePersist>().ResetScenePersist();
        // SceneManager.LoadScene(nextSceneIndex);
        
        //FOR DEMO
        FindObjectOfType<ScenePersist>().ResetScenePersist();
        FindObjectOfType<GameSession>().ResetGameSession();

    }
}
