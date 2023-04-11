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
        private SpriteRenderer mySpriteRenderer;
        [SerializeField] private GameObject characterGO;
        
        [Header("Collision")]
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private LayerMask bounceableLayer;
        [SerializeField] bool isOnGround = false;
        [SerializeField] float groundLengthGizmo = .3f;
        [SerializeField] private Vector3 colliderOffset;
      
        

        [Header("HorizontalMovement")]
        private Vector2 moveInput;
        [SerializeField] private float runSpeed = 10f;
        [SerializeField] private float maxSpeed = 7f;
        [SerializeField] private float linearDrag = 4f;
    
    [Header("JumpingFalling")]
    private float jumpSpeed;
    [SerializeField] private float waterJumpSpeed = 5f;
    [SerializeField] private float groundJumpSpeed = 13f;
    [SerializeField] private float jumpDelay = 0.25f;
    private float jumpTimer;
    [SerializeField] private float playerGravity = 8f;
    [SerializeField] float fallMultiplier = 5f;
    [SerializeField] private float inAirDrag = .5f;
    [SerializeField] private AudioClip jumpSFX;
    [SerializeField] private AudioClip bounceSFX;
    public float SFXVolume;
    
    [Header("Water Physics")]
    [SerializeField] private float waterGravity = .5f;
    [SerializeField] private float waterLinearDrag = 2f;
    
    [Header("PowerUps")] 
    public bool canDoPowerUp = false;
    [SerializeField] private float powerUpTimeDelay = 3f;
    [SerializeField] private string enemyTypeString;

    [SerializeField] private float zipSpeed = 100f;
   // private EnemyType enemyType = new EnemyType();
    
    [Header("Other")]
    private bool isAlive = true;
    [SerializeField] private AudioClip deathSound;



    void Start()
    {
        myRb2d = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        myBodyCollider2D = GetComponent<CapsuleCollider2D>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();

    }

    void Update()
    {
        if (!isAlive){return;}
        CheckIfDead();
    }

    private void FixedUpdate()
    {
        if (!isAlive){return;}
        Run();
        if (jumpTimer > Time.fixedTime && isOnGround)
        {
            DoAJump(); 
        }
        FlipSprite();
        ModifyPhysics();
        CheckIsOnGround();


    }

    void OnMove(InputValue value)
    {
        if (!isAlive){return;}
        moveInput = value.Get<Vector2>();
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
    
    void OnPowerUp(InputValue value)
    {
        if (value.isPressed && canDoPowerUp)
        {
            switch (enemyTypeString)
            {
                case "BlueDoubleJump":
                    myAnimator.SetBool("dblJumpReady", true);
                    DoAJump();
                    myAnimator.SetBool("dblJumpReady", false);
                    break;
                case "RedZipDash":
                    DoADash();
                    break;
                case "YellowHoverGlide":
                    //DoHoverGlide();
                    break;
                case "Normal":
                    break;
                default:
                    Debug.Log("no powerup detected");
                    break;
            }
            mySpriteRenderer.material.color = Color.white;
            enemyTypeString = "";
            canDoPowerUp = false;
            //myAnimator.SetBool("dblJumpReady", false);
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        //if collision is an enemy and enemy is not frozen
        if (col.gameObject.CompareTag("Enemy"));
        {
            if (enemyTypeString == "")
            {
                EnemyMovement enemy = col.gameObject.GetComponent<EnemyMovement>();
                //if (enemy.enemyIsFrozen) {return;}
                EnemyType thisEnemy = enemy.enemyType;
                enemyTypeString = thisEnemy.ToString();
                InitiatePowerUp(enemyTypeString);
            }
            
        }
        
        // if (rightRaycast.collider.IsTouchingLayers(bounceableLayer) || leftRaycast.collider.IsTouchingLayers(bounceableLayer)) 
        if (myBodyCollider2D.IsTouchingLayers(bounceableLayer))
        {
            AudioSource.PlayClipAtPoint(bounceSFX, Camera.main.transform.position, SFXVolume); 
            //Debug.Log("played bounce clip");
        }

    }
    
    public void InitiatePowerUp(string type)
    {
         switch (type)
        {
            case "BlueDoubleJump":
                mySpriteRenderer.material.color = Color.blue;
                break;
            case "RedZipDash":
                mySpriteRenderer.material.color = Color.red;
                break;
            case "YellowHoverGlide":
                mySpriteRenderer.material.color = Color.yellow;
                break;
            default:
                Debug.Log("No power up found");
                break;
        }      
        canDoPowerUp = true;
        StartCoroutine(PowerUpTimerCoroutine());
    }
    IEnumerator PowerUpTimerCoroutine()
    {
        yield return new WaitForSecondsRealtime(powerUpTimeDelay);
        mySpriteRenderer.material.color = Color.white;
        enemyTypeString = "";
        canDoPowerUp = false;
    }



    private void DoAJump()
    { 
        
        // myRb2d.velocity = new Vector2(myRb2d.velocity.x, 0);
        // myRb2d.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        myRb2d.velocity = new Vector2(myRb2d.velocity.x, jumpSpeed);
        jumpTimer = 0;
        AudioSource.PlayClipAtPoint(jumpSFX, Camera.main.transform.position, SFXVolume);
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

    private void DoADash()
    {
        //myRb2d.velocity = new Vector2(0, myRb2d.velocity.y);
        myRb2d.AddForce(new Vector2(transform.localScale.x * zipSpeed, 0), ForceMode2D.Force);
        
    }


    void Run()
    {
        Vector2 playerVelocity = new Vector2(moveInput.x * runSpeed, 0f );
        //myRb2d.velocity = playerVelocity;
       myRb2d.AddForce(playerVelocity);
       
        bool playerHasHorizontalSpeed = Mathf.Abs(myRb2d.velocity.x) > .1; //.1 because mathf.Epsilon was causing issues
        
        if (Mathf.Abs(myRb2d.velocity.x) > maxSpeed)
        {
            myRb2d.velocity = new Vector2(Mathf.Sign(myRb2d.velocity.x) * maxSpeed, myRb2d.velocity.y);
        }
        
        myAnimator.SetBool("isRunning", playerHasHorizontalSpeed);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + colliderOffset, transform.position + colliderOffset + Vector3.down * groundLengthGizmo);
        Gizmos.DrawLine(transform.position - colliderOffset, transform.position - colliderOffset + Vector3.down * groundLengthGizmo);
    }

    void ModifyPhysics()
    {
        // isOnGround = 
        //     Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLengthGizmo, groundLayer ) 
        //     || Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLengthGizmo, groundLayer) 
        //     || Physics2D.Raycast(transform.position + colliderOffset, Vector2.down,groundLengthGizmo, bounceableLayer) 
        //     || Physics2D.Raycast(transform.position - colliderOffset, Vector2.down,groundLengthGizmo, bounceableLayer);
        
        // rightRaycast = Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLengthGizmo);
        // leftRaycast = Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLengthGizmo);
        //
        //  Debug.DrawRay(transform.position + colliderOffset, Vector2.down, Color.red);
        // Debug.DrawRay(transform.position - colliderOffset, Vector2.down, Color.red);
        // //Check Is On Ground
       
        
        
        //DETERMINES IF CHANGING DIRECTIONS
        bool changingDirection = (moveInput.x > 0 && myRb2d.velocity.x < 0) || (moveInput.x < 0 && myRb2d.velocity.x > 0);
        
        
        //check if in water first
        if (myBodyCollider2D.IsTouchingLayers(LayerMask.GetMask("Water")))
        {
            myRb2d.gravityScale = waterGravity;
            myRb2d.drag = waterLinearDrag;
            jumpSpeed = waterJumpSpeed;
            
            myAnimator.SetBool("isOnGround", true);
            
        } else if (isOnGround) //if not in water, check if on ground
        {            
            myAnimator.SetBool("isOnGround", true);
            jumpSpeed = groundJumpSpeed;

            //ADJUSTS LINEAR DRAG IF PLAYER IS CHANGING DIRECTIONS
            if ( Mathf.Abs(moveInput.x) < 0.4f || changingDirection)
            {
                myRb2d.drag = linearDrag;
            }
            else
            {
                myRb2d.drag = 0;
            }
            
            myRb2d.gravityScale = 0;
        }
        else //if not on ground and not in water, (in air?) do this.
        {
            myAnimator.SetBool("isOnGround", false);
            float playerVerticalSpeed = Mathf.Sign(myRb2d.velocity.y);
            myAnimator.SetFloat("Yvelocity", playerVerticalSpeed);
            
            //myRb2d.gravityScale = playerGravity;
            myRb2d.drag = inAirDrag;
            if (myRb2d.velocity.y <= 0)
            {
                myRb2d.gravityScale = playerGravity * fallMultiplier;
            }
            else if (myRb2d.velocity.y > 0 ) //&& !jumpValuePressed)
            {
                myRb2d.gravityScale = playerGravity * (fallMultiplier / 2);
            }

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

    void CheckIfDead()
    {
        if (myBodyCollider2D.IsTouchingLayers(LayerMask.GetMask("Enemies", "Hazards")))
        {
            isAlive = false;
            myAnimator.SetTrigger("isDead");
            StartCoroutine(DeathProcess());
        }
    }

    IEnumerator DeathProcess()
    {
        float alphaVal = mySpriteRenderer.color.a;
        Color tmp = mySpriteRenderer.color;
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


    void CheckIsOnGround()
    {
    
        isOnGround = 
            Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLengthGizmo, groundLayer ) 
            || Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLengthGizmo, groundLayer) 
            || Physics2D.Raycast(transform.position + colliderOffset, Vector2.down,groundLengthGizmo, bounceableLayer) 
            || Physics2D.Raycast(transform.position - colliderOffset, Vector2.down,groundLengthGizmo, bounceableLayer);
        
        
        // if (rightRaycast.rigidbody.IsTouchingLayers(groundLayer) || leftRaycast.rigidbody.IsTouchingLayers(groundLayer) ||
        //     rightRaycast.rigidbody.IsTouchingLayers(bounceableLayer) ||
        //     leftRaycast.rigidbody.IsTouchingLayers(bounceableLayer))
        // {
        //     isOnGround = true;
        //     Debug.Log("is on ground");
        // }
        // else
        // {
        //     isOnGround = false;
        // }
    }
    
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

