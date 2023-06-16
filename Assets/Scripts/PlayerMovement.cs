using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    [Header("Components")]
        private Rigidbody2D myRb2d;
        private Animator myAnimator;
        private CapsuleCollider2D myBodyCollider2D;
        private PlayerManager playerManager;
        private SpriteRenderer mySpriteRenderer;
        //[SerializeField] private GameObject characterGO;

        [Header("Ground")] private CheckOnGround checkOnGround;
        [SerializeField] bool isOnGround;
        
        // [SerializeField] private LayerMask groundLayer;
        [SerializeField] private LayerMask bounceableLayer;
      
        // [SerializeField] float groundLengthGizmo = .3f;
        // [SerializeField] private Vector3 colliderOffset;
      
        

        [Header("HorizontalMovement")]
        private Vector2 moveInput;
        [SerializeField] private float runSpeed;
        [SerializeField] private float maxSpeed = 7f;
        [SerializeField] private float runningLinearDrag = 4f;
        //private float movementSmoothing = .05f;
      //  private Vector3 smoothingVelocity = Vector3.zero;
        private bool isWet = false;

        [Header("Water Physics")]
    [SerializeField] private float waterLinearDrag = .5f;

    [Header("Other")]
    private bool isAlive = true;
    [SerializeField] private AudioClip bounceSFX;
    public float SFXVolume;

    void Start()
    {
        myRb2d = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider2D = GetComponent<CapsuleCollider2D>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        playerManager = GetComponent<PlayerManager>();
        SFXVolume = playerManager.SFXVolume;
        checkOnGround = GetComponent<CheckOnGround>();
    }

    void Update()
    {
        isAlive = playerManager.playerIsAlive;
    }

    private void FixedUpdate()
    {
        if (!isAlive){return;}
        
        //gets isOnGround from other script
        isOnGround = checkOnGround.isOnGround;
        ModifyMovementPhysics();
        Run();
        FlipSprite();
        
    }

    void OnMove(InputValue value)
    {
        if (!isAlive){return;}
        moveInput = value.Get<Vector2>();
    }
    
    private void OnCollisionEnter2D()
    {
        if (myBodyCollider2D.IsTouchingLayers(bounceableLayer))
        {
            AudioSource.PlayClipAtPoint(bounceSFX, Camera.main.transform.position, SFXVolume); 
        }
    }
    
    void Run()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, 0f );
        //myRb2d.velocity = playerVelocity;
       myRb2d.AddForce(playerVelocity);
       
       // Vector2 targetVelocity = new Vector2(moveInput.x * maxSpeed, myRb2d.velocity.y);
       // //I have my maxSpeed in another script, but whatever.
       // // And then smoothing it out and applying it to the character
       // myRb2d.velocity = Vector3.SmoothDamp(myRb2d.velocity, targetVelocity, ref smoothingVelocity , movementSmoothing);
       
        if (Mathf.Abs(myRb2d.velocity.x) > maxSpeed)
        {
            myRb2d.velocity = new Vector2(Mathf.Sign(myRb2d.velocity.x) * maxSpeed, myRb2d.velocity.y);
        }
        
        bool playerHasHorizontalSpeed = Mathf.Abs(myRb2d.velocity.x) > .25; //.1 because mathf.Epsilon was causing issues
        myAnimator.SetBool("isRunning", playerHasHorizontalSpeed);
    }
    

    void ModifyMovementPhysics()
    {

        //DETERMINES IF CHANGING DIRECTIONS
        bool changingDirection = (moveInput.x > 0 && myRb2d.velocity.x < 0) || (moveInput.x < 0 && myRb2d.velocity.x > 0);

        if (isOnGround)
        {
            //set animator to "isOnGround"
            //myAnimator.SetBool("isOnGround", true);

            //check if in water and set linear drag and isWet variable
            if (myBodyCollider2D.IsTouchingLayers(LayerMask.GetMask("Water")))
            {
                isWet = true;
            }
            else
            {
                isWet = false;
            }

            //ADJUSTS LINEAR DRAG IF PLAYER IS CHANGING DIRECTIONS
            if ((Mathf.Abs(moveInput.x) < .4f  ||  changingDirection) && Mathf.Abs(myRb2d.velocity.y) < .2f)
            {
                myRb2d.drag = runningLinearDrag;
            }
            else
            {
                if (isWet)
                {
                   myRb2d.drag = waterLinearDrag; 
                }
                else
                {
                   myRb2d.drag = 0; 
                }
                
                
            }
        }else //if not On Ground
        {
            myRb2d.drag = 0;
        }
        
    }
    void FlipSprite()
    {
        bool playerHasHorizontalSpeed = Mathf.Abs(myRb2d.velocity.x) > .1; //Mathf.Epsilon;

        if (playerHasHorizontalSpeed)
        {
          //mathf sign returns 1 of the positive/negative value of its contents.
          //this alters the transform of the gameobject to flip it
          transform.localScale = new Vector2(Mathf.Sign(myRb2d.velocity.x), 1f);  
        }
        
    }

    // void CheckIfDead()
    // {
    //     if (myBodyCollider2D.IsTouchingLayers(LayerMask.GetMask("Enemies", "Hazards")))
    //     {
    //         isAlive = false;
    //         myAnimator.SetTrigger("isDead");
    //         StartCoroutine(DeathProcess());
    //     }
    // }
    //
    // IEnumerator DeathProcess()
    // {
    //     float alphaVal = mySpriteRenderer.color.a;
    //     Color tmp = mySpriteRenderer.color;
    //     //GetComponent<AudioManager>().StopMusic();
    //     AudioSource.PlayClipAtPoint(deathSound, Camera.main.transform.position, SFXVolume);
    //
    //     while (mySpriteRenderer.color.a > 0)
    //     {
    //         alphaVal -= .1f;
    //         tmp.a = alphaVal;
    //         mySpriteRenderer.color = tmp;
    //
    //         yield return new WaitForSeconds(.1f); // update interval
    //     }
    //     FindObjectOfType<GameSession>().ProcessPlayerDeath();
    //     Destroy(gameObject);
    // }


    // void CheckIsOnGround()
    // {
    //                         //CHANGE TO A BOXCAST? 
    //     // isOnGround = 
    //     //     Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLengthGizmo, groundLayer ) 
    //     //     || Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLengthGizmo, groundLayer) 
    //     //     || Physics2D.Raycast(transform.position + colliderOffset, Vector2.down,groundLengthGizmo, bounceableLayer) 
    //     //     || Physics2D.Raycast(transform.position - colliderOffset, Vector2.down,groundLengthGizmo, bounceableLayer);
    //
    //   
    //     if (isOnGround)
    //     {
    //         isJumping = false;
    //     }
    //     // if (rightRaycast.rigidbody.IsTouchingLayers(groundLayer) || leftRaycast.rigidbody.IsTouchingLayers(groundLayer) ||
    //     //     rightRaycast.rigidbody.IsTouchingLayers(bounceableLayer) ||
    //     //     leftRaycast.rigidbody.IsTouchingLayers(bounceableLayer))
    //     // {
    //     //     isOnGround = true;
    //     //     Debug.Log("is on ground");
    //     // }
    //     // else
    //     // {
    //     //     isOnGround = false;
    //     // }
    // }
    
    // float forwardSpeed;
    // float liftFactor = 0.1f;
    // float dragValue = 0.05f;
    //
    // forwardSpeed = transform.InverseTransformDirection(currentVelocity).z;
    // currentVelocity += gravity * deltaTime; //weight
    // Vector3 liftValue = Vector3.Dot(transform.forward, currentVelocity.normalized) * liftFactor * forwardSpeed; //lift
    // currentVelocity = Vector3.Lerp(currentVelocity, transform.forward * forwardSpeed, liftValue * deltaTime); // thrust
    // currentVelocity *= dragValue * deltaTime; //drag
}

