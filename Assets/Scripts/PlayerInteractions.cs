using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInteractions : MonoBehaviour
{
     [Header("Components")]
        private Rigidbody2D myRb2d;
        private Animator myAnimator;
        //private CapsuleCollider2D myBodyCollider2D;
        private SpriteRenderer mySpriteRenderer;
        //private PlayerJump playerJump;
        private PlayerManager playerManager;
        
    [Header("PowerUps")] 
    public bool canDoPowerUp = false;
    [SerializeField] private float powerUpTimeDelay = 3f;
    public string powerUpString;

    [SerializeField] private float zipSpeed = 1000f;
    [SerializeField] private float doubleJumpSpeed = 12f;
    
    [Header("Other")]
    //public bool playerIsAlive = true;
    [SerializeField] private AudioClip dblJumpSound;
    [SerializeField] private AudioClip zipDashSound;
    private PowerUpActivator thisActivator;
    private float sFXVolume;

    //public bool testCoroutineHappening;
    public float coroutinesHappening = 0f;


    void Start()
    {
        myRb2d = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        playerManager = GetComponent<PlayerManager>();
        sFXVolume = playerManager.SFXVolume;
        //thisActivator = GetComponent<PowerUpActivator>();

    }
    
    void OnPowerUp(InputValue value)
    {
        if (!playerManager.playerIsAlive) { return; }
        
        if (value.isPressed && canDoPowerUp)
        {
            switch (powerUpString)
            {
                case "BlueDoubleJump":
                    myAnimator.SetBool("dblJumpReady", true);
                    // playerJump.DoAJump();
                    // myAnimator.SetBool("dblJumpReady", false);
                    DoubleJump();
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
            //StopCoroutine(PowerUpTimerCoroutine());
            //Debug.Log("powerup Performed");
            
            mySpriteRenderer.material.color = Color.white;
            powerUpString = "";
            canDoPowerUp = false;
            
            //myAnimator.SetBool("dblJumpReady", false);
        }
    }
    
    
//COLLISION STUFF    
    private void OnCollisionEnter2D(Collision2D col)
    {
        //Debug.Log(col.gameObject.tag + " collided with player");
        //if collision is an enemy
        if (col.gameObject.CompareTag("Enemy") )
        {
            Debug.Log("player Interations script: Collided with enemy tag");
            //get enemyMovement script
            thisActivator = col.gameObject.GetComponentInChildren<PowerUpActivator>();
            
            if (thisActivator.bounceOn)   // (powerUpString == ""  && thisActivator.bounceOn)
            {
                thisActivator.TempDeactivate();
                Debug.Log("deactivated activator (attempt)");
                //update the player's enemyTypeString to the enemy's 
                powerUpString = thisActivator.powerUpType.ToString();
                InitiatePowerUp(powerUpString);
            }
        }
    }

    public void InitiatePowerUp(string type)
    {
        canDoPowerUp = true;
        StartCoroutine(PowerUpTimerCoroutine());
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
        }

        coroutinesHappening--;

        //Debug.Log("Coroutine Ended");
    }


    private void DoADash()
    {
         myRb2d.gravityScale = 0;
         myRb2d.drag = 0;
         
         myRb2d.velocity = new Vector2(0, myRb2d.velocity.y);
         
         // FireRay();
         
         //Ray ray = new Ray(transform.position, transform.forward);
         // RaycastHit hitData;
         //RaycastHit2D hitData = Physics2D.Raycast(transform.position, transform.forward, .5f);
         
         myRb2d.AddForce(new Vector2(transform.localScale.x * zipSpeed, 0)); //, ForceMode2D.Force);

         //Physics2D.Raycast(transform.position + new Vector3(.5f, 0), Vector2.down, 1f); 
        
         
         AudioSource.PlayClipAtPoint(zipDashSound, Camera.main.transform.position, sFXVolume);


    }
    
    // void FireRay()
    // {
    //     Ray ray = new Ray(transform.position, transform.forward);
    //     RaycastHit hitData;
    //     Physics.Raycast(ray, out hitData);
    //
    // }

    private void DoubleJump()
    {

        myRb2d.drag = 0;
        AudioSource.PlayClipAtPoint(dblJumpSound, Camera.main.transform.position, sFXVolume);
        // myRb2d.velocity = new Vector2(myRb2d.velocity.x, 0);
        // myRb2d.AddForce(Vector2.up * jumpSpeed, ForceMode2D.Impulse);
        myRb2d.velocity = new Vector2(myRb2d.velocity.x, doubleJumpSpeed);
        


        //jumpTimer = 0;
        //AudioSource.PlayClipAtPoint(dblJumpSound, Camera.main.transform.position, sFXVolume);
    }
    
}
