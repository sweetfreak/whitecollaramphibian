using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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

        [Header("JumpingFalling")]
    private float jumpSpeed;

    [SerializeField] private float maxYVelocity = 10f;
    [SerializeField] private float groundJumpSpeed = 13f;
    [SerializeField] private float jumpDelay = 0.25f;
    private float jumpTimer;
    [SerializeField] private float playerGravity = 8f;
    [SerializeField] float fallMultiplier = 5f;
    [SerializeField] private AudioClip jumpSFX;
    private float sFXVolume;

    [Header("Water Physics")] 
    private bool isWet = false;
    [SerializeField] private float waterJumpSpeed = 5f;
    [SerializeField] private float waterGravity = 1f;


    [Header("Other")]
    private bool isAlive = true;



    void Start()
    {
        myRb2d = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider2D = GetComponent<CapsuleCollider2D>();
        playerManager = GetComponent<PlayerManager>();
        sFXVolume = playerManager.SFXVolume;
        waterGravity = playerGravity/2;
    }

    private void FixedUpdate()
    {
        isAlive = playerManager.playerIsAlive;
        if (!isAlive){return;}
        
        //gets isOnGround from other script
        isOnGround = GetComponent<CheckOnGround>().isOnGround;

        ModifyJumpPhysics();
        
        //if player is on ground, or recently hit the jump button, DO A JUMP
        if (jumpTimer > Time.fixedTime && isOnGround)
        {
            DoAJump(); 
        }
    }


    void OnJump(InputValue value)
    {
        //IF NOT ALIVE, RETURN
        if (!isAlive){return;}
        
        //IF INPUT SYSTEM JUMP BUTTON IS PRESSED, ADD FORCE TO PLAYER
        if (value.isPressed)
        { 
            //for jump delay (see fixed update to call DoAJump() )
            jumpTimer = Time.time + jumpDelay;
            
        }
    }
    
    public void DoAJump()
    { 
        myRb2d.drag = 0;
        // myRb2d.velocity = new Vector2(myRb2d.velocity.x, 0);
        // myRb2d.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        myRb2d.velocity = new Vector2(myRb2d.velocity.x, jumpSpeed);
        jumpTimer = 0;
        AudioSource.PlayClipAtPoint(jumpSFX, Camera.main.transform.position, sFXVolume);
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
    
    
    void ModifyJumpPhysics()
    {
        //check if in water first
        if (myBodyCollider2D.IsTouchingLayers(LayerMask.GetMask("Water")))
        {
            isWet = true;
            jumpSpeed = waterJumpSpeed;
        } else {
            isWet = false;
            jumpSpeed = groundJumpSpeed;
        }

        if (isWet)
        {
            myRb2d.gravityScale = waterGravity;
            return;
        }
        
        if (isOnGround)
        {
            myRb2d.gravityScale = 0;
            myAnimator.SetBool("dblJumpReady", false);
        }
        else
        {
            float playerVerticalSpeed = Mathf.Sign(myRb2d.velocity.y);
            if (Mathf.Abs(myRb2d.velocity.y) > maxYVelocity)
            {
                myRb2d.velocity = new Vector2(myRb2d.velocity.x, Mathf.Sign(myRb2d.velocity.y) * maxYVelocity);
            }

            myAnimator.SetBool("isOnGround", false);
            myAnimator.SetFloat("Yvelocity", playerVerticalSpeed);
            
            
            
            // if (isWet)
            // {
            //    myRb2d.gravityScale = waterGravity;
            //     return;
            // }

            if (playerVerticalSpeed > 0)
            {
                myRb2d.gravityScale = playerGravity ;
            }
            else if (playerVerticalSpeed < 0 )
            {
                myRb2d.gravityScale = playerGravity * fallMultiplier;
            }

        }
        
    }
}
