using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerMovement : MonoBehaviour
{
    [Header("Components")] private Rigidbody2D myRb2d;
    private Animator myAnimator;
    private BoxCollider2D myBodyCollider2D;
    private PlayerManager playerManager;
    private PlayerInteractions playerInteractions;

    private SpriteRenderer mySpriteRenderer;
    //[SerializeField] private GameObject characterGO;

    [Header("Ground")] private CheckOnGround checkOnGround;
    [SerializeField] bool isOnGround;

    // [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask bounceableLayer;

    // [SerializeField] float groundLengthGizmo = .3f;
    // [SerializeField] private Vector3 colliderOffset;



    [Header("HorizontalMovement")] 
    [HideInInspector] public Vector2 moveInput;
    [SerializeField] private float runSpeed;
    [SerializeField] private float runMaxSpeed = 7f;
    [SerializeField] public float airSpeed;
    [SerializeField] private float airMaxSpeed = 15f;
    [SerializeField] private float maxSpeed = 7f;

    public float runningLinearDrag;

    //private float movementSmoothing = .05f;
    //  private Vector3 smoothingVelocity = Vector3.zero;
    private bool isInWater = false;

    [Header("Water Physics")] [SerializeField]
    private float waterLinearDrag = .5f;

    [Header("Other")] 
    private bool isAlive = true;
    [SerializeField] private AudioClip bounceSFX;
    public float bounceVolume = .5f;

    void Start()
    {
        myRb2d = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider2D = GetComponent<BoxCollider2D>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        playerManager = FindObjectOfType<PlayerManager>();
        playerInteractions = FindObjectOfType<PlayerInteractions>();
        checkOnGround = GetComponent<CheckOnGround>();
    }

    void Update()
    {
        isAlive = playerManager.playerIsAlive;
    }

    private void FixedUpdate()
    {
        if (!isAlive)
        {
            return;
        }

        //gets isOnGround from other script
        if (!playerInteractions.isDashing)
        {
          isOnGround = checkOnGround.isOnGround;
                  isInWater = checkOnGround.isWet;
                  ModifyMovementPhysics();
                  Run();
                  FlipSprite();  
        }
        


    }

    // void OnMove(InputValue value)
    // {
    //     if (!isAlive){return;}
    //     moveInput = value.Get<Vector2>();
    // }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!isAlive)
        {
            return;
        }

        moveInput = (context.ReadValue<Vector2>());
    }

    private void OnCollisionEnter2D()
    {
        if (myBodyCollider2D.IsTouchingLayers(bounceableLayer))
        {
           // Debug.Log("THIS IS PLAYING)");
            AudioSource.PlayClipAtPoint(bounceSFX, Camera.main.transform.position, bounceVolume);
        }
    }

    void Run()
    {
        Vector2 playerVelocity;
        if (!isOnGround && !isInWater)
        {
            playerVelocity = new Vector2(moveInput.x * airSpeed, 0f);
            //Debug.Log(playerVelocity);
            //maxSpeed = airMaxSpeed;
        }
        else
        {
            playerVelocity = new Vector2(moveInput.x * runSpeed, 0f);
            //Debug.Log(playerVelocity);
            //maxSpeed = runMaxSpeed;

        }

        myRb2d.AddForce(playerVelocity);
        //myRb2d.velocity = playerVelocity;

        // Vector2 targetVelocity = new Vector2(moveInput.x * maxSpeed, myRb2d.velocity.y);
        // //I have my maxSpeed in another script, but whatever.
        // // And then smoothing it out and applying it to the character
        // myRb2d.velocity = Vector3.SmoothDamp(myRb2d.velocity, targetVelocity, ref smoothingVelocity , movementSmoothing);

        if (Mathf.Abs(myRb2d.velocity.x) > maxSpeed)
        {
            myRb2d.velocity = new Vector2(Mathf.Sign(myRb2d.velocity.x) * maxSpeed, myRb2d.velocity.y);
        }

        bool playerHasHorizontalSpeed =
            Mathf.Abs(myRb2d.velocity.x) > .25; //.1 because mathf.Epsilon was causing issues
        myAnimator.SetBool("isRunning", playerHasHorizontalSpeed);
    }


    void ModifyMovementPhysics()
    {

        //DETERMINES IF CHANGING DIRECTIONS
        bool changingDirection =
            (moveInput.x > 0 && myRb2d.velocity.x < 0) || (moveInput.x < 0 && myRb2d.velocity.x > 0);

        if (isOnGround)
        {
            //set animator to "isOnGround"
            //myAnimator.SetBool("isOnGround", true);


            //ADJUSTS LINEAR DRAG IF PLAYER IS CHANGING DIRECTIONS
            if ((Mathf.Abs(moveInput.x) < .4f || changingDirection) && Mathf.Abs(myRb2d.velocity.y) < .2f)
            {
                myRb2d.drag = runningLinearDrag;
            }
            else
            {
                if (isInWater)
                {
                    myRb2d.drag = waterLinearDrag;
                }
                else
                {
                    myRb2d.drag = 0;
                }


            }
        }
        else //if not on ground
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
}