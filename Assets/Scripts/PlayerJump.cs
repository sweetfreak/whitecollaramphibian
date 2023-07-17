using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerJump : MonoBehaviour
{
     [Header("Components")]
        private Rigidbody2D myRb2d;
        private Animator myAnimator;
        private CapsuleCollider2D myBodyCollider2D;
        [SerializeField] private GameObject characterGO;
        private PlayerManager playerManager;
        
        [Header("Collision")]
        private bool isOnGround = false;

        [Header("InputActions")] private bool desiredJump;
        private bool pressingJump;

        [Header("JumpingFalling")]
    private float jumpSpeed;

    [SerializeField] private float maxYVelocity = 10f;
    [SerializeField] private float normalJumpSpeed = 13f;
    [SerializeField] private float jumpDelay = 0.25f;
    private float jumpTimer;

    [SerializeField] private float playerGravity;
    [SerializeField] private float regularGravity = 8f;
    [SerializeField] float fallMultiplier = 5f;
    [SerializeField] private AudioClip jumpSFX;
    [SerializeField] private float jumpVolume = .5f;
    public bool canDoubleJump;
    public float doubleJumpSpeed;
    public AudioClip dblJumpSFX;

    [Header("Water Physics")] 
    [SerializeField] private AudioClip waterJumpSFX;
    [SerializeField] private bool isInWater = false;
    [SerializeField] private float waterJumpSpeed = 5f;
    [SerializeField] private float waterGravity = 1f;


    [Header("Other")]
    private bool isAlive = true;

    private PlayerInteractions playerInteractions;
    



    void Start()
    {
        myRb2d = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider2D = GetComponent<CapsuleCollider2D>();
        playerManager = GetComponent<PlayerManager>();
        playerInteractions = GetComponent<PlayerInteractions>();
        waterGravity = regularGravity/2;
    }
    
    public void OnJump(InputAction.CallbackContext context)
    {
        //IF NOT ALIVE, RETURN
        if (!isAlive)
        {
            
            return;
        }

        //IF INPUT SYSTEM JUMP BUTTON IS PRESSED, ADD FORCE TO PLAYER
        if (context.started)
        {
            desiredJump = true;
            pressingJump = true;
            // Debug.Log("context started");
            //for jump delay (see fixed update to call DoAJump() )
            // Debug.Log("Setting jumptimer: " + jumpTimer);
            jumpTimer = Time.fixedTime + jumpDelay;
        }

        if (context.canceled)
        {
            pressingJump = false;
            //desiredJump = true; //this is redundant
            // Debug.Log("context cancelled");
        }
        
        //ADDED THIS CHUNK
        //for jump delay (see fixed update to call DoAJump() )
        // Debug.Log("Setting jumptimer: " + jumpTimer);
        // jumpTimer = Time.fixedTime + jumpDelay;
        
        
        ChooseJump();
    }

    private void FixedUpdate()
    {
        isAlive = playerManager.playerIsAlive;
        if (!isAlive){return;}

        if (!playerInteractions.isDashing)
        {
           myRb2d.gravityScale = playerGravity;
           
                   
                   //gets isOnGround from other script
                   isOnGround = GetComponent<CheckOnGround>().isOnGround;
                   isInWater = GetComponent<CheckOnGround>().isWet;
                   canDoubleJump = playerInteractions.canDoubleJump;
                   ModifyJumpPhysics();
                   

                   //if player is on ground and recently hit the jump button, DO A JUMP
                   if ((jumpTimer > Time.fixedTime) && isOnGround)
                   {
                       // Debug.Log("jumptimer: " + jumpTimer);
                       // Debug.Log("time.fixedtime: " + Time.fixedTime);
                       // Debug.Log("jumpTimer was less than time.fixedtime");
                       DoAJump(jumpSFX, normalJumpSpeed); 
                   }  
        }
        
    }

    // public void OnJump(InputAction.CallbackContext context)
    // {
    //     //IF NOT ALIVE, RETURN
    //     if (!isAlive)
    //     {
    //         
    //         return;
    //     }
    //
    //     //IF INPUT SYSTEM JUMP BUTTON IS PRESSED, ADD FORCE TO PLAYER
    //     if (context.started)
    //     {
    //         desiredJump = true;
    //         pressingJump = true;
    //        // Debug.Log("context started");
    //     }
    //
    //     if (context.canceled)
    //     {
    //         pressingJump = false;
    //         desiredJump = true;
    //         // Debug.Log("context cancelled");
    //     }
    //     
    //     ChooseJump();
    // }

    //new version!!
    public void ChooseJump()
    {
        
        
        //added pressing jump for all this jump stuff
        if (!isOnGround && playerInteractions.canDoubleJump && pressingJump)
        {
            DoubleJump();
        }
        else if (isInWater)// && desiredJump)
        {
            // CHANGE 
            //TO
            //WATER OR
            //SWIMMING
            //SFX
            DoAJump(waterJumpSFX, waterJumpSpeed);
        }
        // else if (desiredJump && isOnGround)
        // {
        //     //added above desiredJump for input actions
        //
        //     
        // }
    }
    
    //  public void ChooseJump()
    // {
    //     //added pressing jump for all this jump stuff
    //     if (!isOnGround && playerInteractions.canDoubleJump && pressingJump)
    //     {
    //         DoubleJump();
    //     }
    //     else if (isInWater && desiredJump)
    //     {
    //         // CHANGE 
    //         //TO
    //         //WATER OR
    //         //SWIMMING
    //         //SFX
    //         DoAJump(waterJumpSFX, waterJumpSpeed);
    //     }
    //     else if (desiredJump && isOnGround)
    //     {
    //         //added above desiredJump for input actions
    //
    //         //for jump delay (see fixed update to call DoAJump() )
    //         Debug.Log("Setting jumptimer: " + jumpTimer);
    //         jumpTimer = Time.fixedTime + jumpDelay;
    //     }
    // }


    // void OnJump(InputValue value)
    // {
    //     //IF NOT ALIVE, RETURN
    //     if (!isAlive){return;}
    //     
    //     //IF INPUT SYSTEM JUMP BUTTON IS PRESSED, ADD FORCE TO PLAYER
    //     if (value.isPressed)
    //     {
    //         if (!isOnGround && canDoubleJump)
    //         { 
    //             myAnimator.SetBool("dblJumpReady", true);
    //             DoAJump(dblJumpSFX);
    //             Debug.Log("double jump!");
    //             playerInteractions.ResetPowerUp();
    //             canDoubleJump = false;
    //         }
    //         else if (isWet)
    //         {
    //             // CHANGE 
    //             //TO
    //             //WATER OR
    //             //SWIMMING
    //             //SFX
    //             DoAJump(waterJumpSFX);
    //         } else
    //         {
    //           //for jump delay (see fixed update to call DoAJump() )
    //           jumpTimer = Time.time + jumpDelay;  
    //         }
    //         
    //     }
    // }
    
    public void DoAJump(AudioClip jumpSound, float jumpSpeed)
    {
        if (!isInWater)
        {
            myRb2d.drag = 0;
        }
       
        myRb2d.velocity = new Vector2(myRb2d.velocity.x, jumpSpeed);
        
        jumpTimer = 0;
        AudioSource.PlayClipAtPoint(jumpSound, Camera.main.transform.position, jumpVolume);
        StartCoroutine(JumpSqueeze(0.5f, 1.2f, 0.1f));

    }

    IEnumerator JumpSqueeze(float xSqueeze, float ySqueeze, float seconds) {
        Vector3 originalSize = transform.localScale;
        Vector3 newSize = new Vector3(xSqueeze * Mathf.Sign(transform.localScale.x), ySqueeze, originalSize.z);
        float t = 0f;
        while (t <= 1.0) {
            t += Time.deltaTime / seconds;
            characterGO.transform.localScale = Vector3.Lerp(originalSize, newSize, t);
            yield return null;
        }
        t = 0f;
        while (t <= 1.0) {
            t += Time.deltaTime / seconds;
            characterGO.transform.localScale = Vector3.Lerp(newSize, originalSize, t);
            yield return null;
        }
    }

    public void DoubleJump()
    {
        myAnimator.SetBool("dblJumpReady", true);
        DoAJump(dblJumpSFX, doubleJumpSpeed);
        Debug.Log("double jump!");
        playerInteractions.ResetPowerUp();
        canDoubleJump = false;
    }

    void ModifyJumpPhysics()
    {
        

        if (isInWater)
        {
            playerGravity = waterGravity;
            //Debug.Log("frog in water");
            return;
        }
        //Debug.Log("Is not in water");
        //jumpSpeed = groundJumpSpeed;
        //Debug.Log("frog out of water");
        
        if (isOnGround)
        {
            //added desired jump for input actions
            desiredJump = false;
            playerGravity = 0;
            myAnimator.SetBool("dblJumpReady", false);
        }
        else 
        {
            float playerVerticalDirection = Mathf.Sign(myRb2d.velocity.y);
            
            if (Mathf.Abs(myRb2d.velocity.y) > maxYVelocity)
            {
                
                myRb2d.velocity = new Vector2(myRb2d.velocity.x, Mathf.Sign(myRb2d.velocity.y) * maxYVelocity);
            }

            myAnimator.SetBool("isOnGround", false);
            myAnimator.SetFloat("Yvelocity", playerVerticalDirection);

            if (!pressingJump) 
            {
                playerGravity = regularGravity * fallMultiplier;
            } 
            else {

                if (playerVerticalDirection > 0)
                {

                    playerGravity = regularGravity;

                }
                else if (playerVerticalDirection < 0)
                {

                    playerGravity = regularGravity * fallMultiplier;
                }
            }
        }
    }
}

