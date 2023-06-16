using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{

    
    [Header("Components")]
    // private Rigidbody2D myRb2d;
    private Animator myAnimator;
    private CapsuleCollider2D myBodyCollider2D;
    private SpriteRenderer mySpriteRenderer;
    private GameObject playerGameObject;
    private CheckPointManager checkPointManager;

    private bool gameHasStarted = false;
    //private Vector3 playerSpawnPoint  
        
    
    
    [Header("Other")]
    [SerializeField] private AudioClip deathSound;
    public float SFXVolume;
    public bool playerIsAlive = true;

    private GameSession gameSession;

    
    [SerializeField] private Vector3 spawnPoint = new Vector3();
    
    //can delete after tests:
    //[SerializeField] private bool updatePosition = false;
    

    private void Awake()
    {
        gameSession = FindObjectOfType<GameSession>();
    }

    private void Start()
    {
        myBodyCollider2D = GetComponent<CapsuleCollider2D>();
        myAnimator = GetComponent<Animator>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!playerIsAlive) {return;}
        CheckIfDead();
    }
    void LateUpdate()
    {
        if (!gameHasStarted)
        { 
            checkPointManager = FindObjectOfType<CheckPointManager>();
            UpdateSpawnPoint(checkPointManager.currentSpawnPoint);
            gameHasStarted = true;
        }
    }

    void OnPause(InputValue value)
    {
        //if (!playerIsAlive){return;}
        Debug.Log("Pause pressed");

        if (gameSession != null)
        {
            gameSession.pausePressed = value.isPressed;
        }
    }
    
    public void UpdateSpawnPoint(Vector3 spawnTransform)
    {
        spawnPoint = spawnTransform;
        gameObject.transform.position = spawnPoint;
    }
    
    //DEATH STUFF
    private void CheckIfDead()
    {
        
        
        if (myBodyCollider2D.IsTouchingLayers(LayerMask.GetMask("Enemies", "Hazards"))  )
        {
            
            playerIsAlive = false;
            myAnimator.SetTrigger("isDead");
            StartCoroutine(DeathProcess());
        }
    }

    IEnumerator DeathProcess()
    {
        float alphaVal = mySpriteRenderer.color.a;
        Color tmp = Color.white;
        //GetComponent<AudioManager>().StopMusic();
        AudioSource.PlayClipAtPoint(deathSound, Camera.main.transform.position, SFXVolume);

        while (mySpriteRenderer.color.a > 0)
        {
            alphaVal -= .1f;
            tmp.a = alphaVal;
            mySpriteRenderer.color = tmp;
 
            yield return new WaitForSeconds(.1f); // update interval
        }
        FindObjectOfType<GameSession>().ProcessPlayerDeath();
        Destroy(gameObject);
    }

}
