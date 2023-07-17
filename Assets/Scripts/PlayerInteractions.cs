using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInteractions : MonoBehaviour
{
     [Header("Components")]
        private Rigidbody2D myRb2d;
        private Animator myAnimator;
        private SpriteRenderer mySpriteRenderer;
        private PlayerManager playerManager;
        private PlayerMovement playerMovement;
        private PlayerJump playerJump;
        
    [Header("PowerUps")] 
    public bool canDoPowerUp = false;
    [SerializeField] private float powerUpTimeDelay = 3f;
    public string powerUpString;


    //[SerializeField] private float doubleJumpSpeed = 12f;
    public bool canDoubleJump;
    //private PlayerMovement playerMovement;

    [Header("Dashing")]
    //private bool canDash = false;
    [SerializeField]
    private float maxXVelocity = 70f;
    public bool isDashing;
    [SerializeField] private float zipSpeed = 100f;
    private float dashTime = 0.2f;
    private float dashingCooldown = 1f;
    [SerializeField] private TrailRenderer trailRenderer;
    
    [Header("Other")]
    //public bool playerIsAlive = true;
    [SerializeField] private AudioClip dblJumpSound;
    [SerializeField] private AudioClip powerUpEarnSFX;
    // [SerializeField] private float powerupEarnVol = 1f;
    [SerializeField] private AudioClip zipDashSound;
    private PowerUpActivator thisActivator;
    [SerializeField] private float dashVolume = .5f;
    //bool boosting = false;
    //[SerializeField] private float boostNum = 10f;
    
    //public bool testCoroutineHappening;
    public float coroutinesHappening = 0f;


    void Start()
    {
        myRb2d = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        playerManager = GetComponent<PlayerManager>();
        playerMovement = GetComponent<PlayerMovement>();
        trailRenderer = FindObjectOfType<TrailRenderer>();
        //playerMovement = FindObjectOfType<PlayerMovement>();
        //thisActivator = GetComponent<PowerUpActivator>();
        playerJump = FindObjectOfType<PlayerJump>();

    }

    void Update()
    {
        // if (Mathf.Abs(myRb2d.velocity.x) > maxXVelocity)
        // {
        //         
        //     myRb2d.velocity = new Vector2(Mathf.Sign(myRb2d.velocity.x) * maxXVelocity, myRb2d.velocity.y);
        // }

        if (isDashing)
        {
            Debug.Log("player X velocity during dash is: " + myRb2d.velocity.x);
        }
        
    }
    
    
    public void OnPowerUp(InputAction.CallbackContext context)
    {
        if (!playerManager.playerIsAlive) { return; }
        
       
        if (context.started && canDoPowerUp)
        {
           // bool skipReset = false;
            
            switch (powerUpString)
            {
                case "BlueDoubleJump":
                    //playerJump.DoAJump(playerJump.dblJumpSFX, playerJump.doubleJumpSpeed);
                playerJump.DoubleJump();
                canDoubleJump = false;
                //skipReset = true;
                break;
                case "YellowZipDash":
                    DoADash();
                    break;
                case "MagentaHoverGlide":
                    //DoHoverGlide();
                    break;
                case "Normal":
                    break;
                default:
                    Debug.Log("no powerup detected");
                    break;
            }
           
            
            //since Jump button is used for power up, need to skip this.
            // if (!skipReset)
            // { 
                ResetPowerUp();
                
            // }
            // skipReset = false;
            

        }
    }

    public void ResetPowerUp()
    {
        mySpriteRenderer.material.color = Color.white;
        powerUpString = "";
        canDoPowerUp = false;
        canDoubleJump = false;
    }


    //COLLISION STUFF    
    private void OnCollisionEnter2D(Collision2D col)
    {
        //Debug.Log(col.gameObject.tag + " collided with player");
        //if collision is an enemy
        if (col.gameObject.CompareTag("Enemy") )
        {
            //get enemyMovement script
            thisActivator = col.gameObject.GetComponentInChildren<PowerUpActivator>();
            
            if (thisActivator.bounceOn)   // (powerUpString == ""  && thisActivator.bounceOn)
            {
                
                Debug.Log("deactivated activator (attempt)");
                //update the player's enemyTypeString to the enemy's 
                powerUpString = thisActivator.powerUpType.ToString();
                InitiatePowerUp(powerUpString);
                
                //thisActivator.TempDeactivate();
            }
        }
    }

    public void InitiatePowerUp(string type)
    {
        canDoPowerUp = true;
        StartCoroutine(PowerUpTimerCoroutine());
        AudioSource.PlayClipAtPoint(powerUpEarnSFX, Camera.main.transform.position, dashVolume);
        ParticleSystem particle;
        switch (type)
        {
            case "BlueDoubleJump":
                mySpriteRenderer.material.color = Color.blue;
                canDoubleJump = true;
                
                
                particle = mySpriteRenderer.GetComponentInChildren<ParticleSystem>();
                particle.startColor = Color.blue;
                particle.Play();
                
                //AudioSource.PlayClipAtPoint(powerUpEarnSFX, Camera.main.transform.position, dashVolume);

                break;
            case "YellowZipDash":
                mySpriteRenderer.material.color = new Color(1f, .35f, 0f, 1);
                particle = mySpriteRenderer.GetComponentInChildren<ParticleSystem>();
                particle.startColor = new Color(1f, .35f, 0f, 1);
                particle.Play();


                break;
            // case "MagentaHoverGlide":
            //     mySpriteRenderer.material.color = Color.magenta;
            //     //AudioSource.PlayClipAtPoint(powerUpEarnSFX, Camera.main.transform.position, dashVolume);
            //     break;
            case "Normal":
                canDoPowerUp = false;
                //StopCoroutine(PowerUpTimerCoroutine());
                break;
            default:
                Debug.Log("No power up found");
                break;
        }
        //Debug.Log("Color swap happened ");

    }
    
    IEnumerator PowerUpTimerCoroutine()
    {
        coroutinesHappening++;
        yield return new WaitForSecondsRealtime(powerUpTimeDelay);

        if (coroutinesHappening == 1)
        {
            mySpriteRenderer.material.color = Color.white;
                    powerUpString = "";
                    canDoPowerUp = false;
                    canDoubleJump = false;
                    //playerMovement.canDoubleJump;
        }

        coroutinesHappening--;

        //Debug.Log("Coroutine Ended");
    }


    private void DoADash()
    {
        AudioSource.PlayClipAtPoint(zipDashSound, Camera.main.transform.position, dashVolume);
        StartCoroutine(DashCoroutine());
        
        //float momentumIncreaser = 0;
        //while the boostNumber is more than 0, add Xforce to the rigidbody.
        //     myRb2d.gravityScale = 0;
        //     myRb2d.drag = 0;
        //     myRb2d.velocity = new Vector2(0, myRb2d.velocity.y);
        //     
        //     myRb2d.AddForce(new Vector2(transform.localScale.x * zipSpeed, 0), ForceMode2D.Force);
        //
        // }
        
    }

    private IEnumerator DashCoroutine()
    {
        
        
        //THE DASH IS TOO FAST/TOO MUCH IN THE AIR!!!!!
        
        
        //canDash = false;
        isDashing = true;
        //float originalGravity = myRb2d.gravityScale;
        //myRb2d.gravityScale = 0;
        myRb2d.drag = playerMovement.runningLinearDrag;
        myRb2d.velocity = new Vector2(0, myRb2d.velocity.y);
        myRb2d.velocity = new Vector2(transform.localScale.x * zipSpeed, 0);
        trailRenderer.emitting = true;
        yield return new WaitForSeconds(dashTime);
        trailRenderer.emitting = false;
        //myRb2d.gravityScale = originalGravity;
        isDashing = false;
        myRb2d.velocity = new Vector2(playerMovement.moveInput.x * playerMovement.airSpeed, myRb2d.velocity.y);
        myRb2d.drag = 0;
        // yield return new WaitForSeconds(dashingCooldown);
        // canDash = true;
    }
    
    // void FireRay()
    // {
    //     Ray ray = new Ray(transform.position, transform.forward);
    //     RaycastHit hitData;
    //     Physics.Raycast(ray, out hitData);
    //
    // }

    // private void DoubleJump()
    // {
    //
    //     // canDoubleJump = true;
    //
    //     myRb2d.drag = 0;
    //     AudioSource.PlayClipAtPoint(dblJumpSound, Camera.main.transform.position, sFXVolume);
    //     // myRb2d.velocity = new Vector2(myRb2d.velocity.x, 0);
    //     // myRb2d.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
    //     myRb2d.velocity = new Vector2(myRb2d.velocity.x, doubleJumpSpeed);
    //     
    //
    //
    //     //jumpTimer = 0;
    //     //AudioSource.PlayClipAtPoint(dblJumpSound, Camera.main.transform.position, sFXVolume);
    // }
    
}
