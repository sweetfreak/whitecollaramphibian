using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public enum EnemyModeType
{
    Idle,
    Moving,
    Chasing,
    Nothing
}

public class EnemyMovement : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D enemyRigidbody;
    [SerializeField] private Animator enemyAnimator;
    //[SerializeField] private BoxCollider2D enemyCollider2D;
    
    
    
    [Header("Movement")]
    public float thawTime;
    private SpriteRenderer enemySpriteRenderer;
    private float enemyFreezeSpeed = 0;
    public bool enemyIsFrozen = false;
    private float enemySpeed = 1f;
    public bool moving;
    [SerializeField] float enemyMoveSpeed = 1f;
    
    public bool chase;
    [SerializeField] private float enemyChasingSpeed = 3f;
    public EnemyModeType currentMoveType;
    private EnemyModeType previousModeType = EnemyModeType.Nothing;
    
    [Header("PowerUp Stuff")]
    private PowerUpType thisPowerUp;
    private PowerUpActivator powerUpActivator;
    private bool hasPowerUpType = false;


    [Header("Enemy Edge Collision")]
    [SerializeField] float colliderDetectDistance = .5f;
    [SerializeField] private Vector3 collisionDetectionOffset;
    [SerializeField] private Vector3 cliffDetectionOffset;
    private Vector2 collisionDirection = Vector2.right;
    [SerializeField] private bool groundDetected;
    [SerializeField] private bool collisionDetected;
    [SerializeField] private LayerMask edgeLayer;
    

    void Start()
    {
        
        enemyRigidbody = GetComponent<Rigidbody2D>();
        enemyAnimator = GetComponent<Animator>();
        enemySpriteRenderer = GetComponent<SpriteRenderer>();
        powerUpActivator = GetComponentInChildren<PowerUpActivator>();
        //enemyCollider2D = GetComponent<BoxCollider2D>();
        thisPowerUp = powerUpActivator.powerUpType;
        GetEnemyType(thisPowerUp);
        ChangeEnemyMode(currentMoveType);
        //BELOW: trying to make it so they go in the direction they're facing from the start...
        //but right now, their ground detection doesn't move with them.
        //enemySpeed *= gameObject.transform.localScale.x;
    }

    void Update()
    {
        CheckForEdges();
        if (currentMoveType != EnemyModeType.Idle)
        {
           MoveEnemy(); 
        }
        // if (thisPowerUp != PowerUpType.Normal && !enemyIsFrozen)
        // {
        //     GetEnemyType(thisPowerUp);
        // }

        
    }

    void ChangeEnemyMode(EnemyModeType thisType)
    {
        // if (thisType == previousModeType)
        // {
        //     return;
        // }
        // previousModeType = thisType;
        
        switch (thisType)
        {
            case EnemyModeType.Idle:
                
                IdleEnemy();
                break;
            case EnemyModeType.Moving:
                MovingEnemy();
                break;
            case EnemyModeType.Chasing:
                ChasingEnemy();
                break;
            default:
                break;
            
        }
        
    }
    

    void IdleEnemy()
    {
        enemyAnimator.SetBool("moving", false);

        enemyRigidbody.bodyType = RigidbodyType2D.Static;
    }
    void MovingEnemy()
    {
        enemyAnimator.SetBool("chasing", false);
        enemyAnimator.SetBool("moving", true);
        enemySpeed = enemyMoveSpeed;
        enemyRigidbody.bodyType = RigidbodyType2D.Kinematic;
        // enemyRigidbody.velocity = new Vector2(enemySpeed, 0);
    }

    void ChasingEnemy()
    {
        enemyAnimator.SetBool("chasing", true);
        enemySpeed = enemyChasingSpeed;
        enemyRigidbody.bodyType = RigidbodyType2D.Kinematic;
        // enemyRigidbody.velocity = new Vector2(enemySpeed, 0);
    }

    void MoveEnemy()
    {
        // if (enemyRigidbody.bodyType != RigidbodyType2D.Static)
        // {
            enemyRigidbody.velocity = new Vector2(enemySpeed, 0);
        // }

    }

    void CheckForEdges()
    {
        
        collisionDetected = Physics2D.Raycast(transform.position + collisionDetectionOffset, collisionDirection, colliderDetectDistance, edgeLayer);
        groundDetected =  Physics2D.Raycast(transform.position + cliffDetectionOffset, Vector2.down, colliderDetectDistance, edgeLayer);

        if ((collisionDetected || !groundDetected))
        {
            // if (!chase)
            // {
            enemySpeed = -enemySpeed;
            
            collisionDirection *= new Vector2(-1, 0);
            cliffDetectionOffset *= new Vector2(-1, 1);
            FlipEnemyFacing();
            // }
            // else if (chase)
            //         {
            //             Debug.Log("enemy could fall off edge?");
            //         }
        }
        
    }
    private void OnDrawGizmos()
    {
        // Gizmos.color = Color.red;
        // Gizmos.DrawLine(transform.position + collisionDetectionOffset, transform.position + collisionDetectionOffset + Vector3.right * colliderDetectDistance);
        Debug.DrawRay(transform.position + collisionDetectionOffset, collisionDirection, Color.red);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position + cliffDetectionOffset, transform.position + cliffDetectionOffset + Vector3.down * colliderDetectDistance);

        // Debug.DrawRay(transform.position + cliffDetectionOffset, Vector2.down, Color.blue);
        
    }

     public void Freeze()
     {
         enemyIsFrozen = true;
         StartCoroutine(JumpSqueeze(1.2f, .5f, 0.1f));
         StartCoroutine(ThawTimeCoroutine());
     }

     public IEnumerator ThawTimeCoroutine()
     {
         enemyAnimator.enabled = false;
         //enemyCollider2D.enabled = false;
         gameObject.layer = LayerMask.NameToLayer("Frozen");
         enemyRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
         enemySpriteRenderer.material.color = Color.grey;
         yield return new WaitForSecondsRealtime(thawTime);
         enemyAnimator.enabled = true;
         //enemyCollider2D.enabled = true;
         gameObject.layer = LayerMask.NameToLayer("Enemies");
         enemyIsFrozen = false;
         enemySpriteRenderer.material.color = Color.white;
         GetEnemyType(thisPowerUp);
         enemyRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
     }
     
    void FlipEnemyFacing()
    {
        if (Mathf.Sign(enemyRigidbody.velocity.x) > 0)
        {
            transform.localScale = new Vector2(1, 1);
        }
        else if (Mathf.Sign(enemyRigidbody.velocity.x) < 0)
        {
            transform.localScale = new Vector2(-1, 1);
        }
    }
    
    public IEnumerator JumpSqueeze(float xSqueeze, float ySqueeze, float seconds) {
        Vector3 originalSize = transform.localScale;
        Vector3 newSize = new Vector3(xSqueeze * Mathf.Sign(originalSize.x), ySqueeze, originalSize.z);
        float t = 0f;
        while (t <= 1.0) {
            t += Time.deltaTime / seconds;
            enemySpriteRenderer.transform.localScale = Vector3.Lerp(originalSize, newSize, t);
            yield return null;
        }
        t = 0f;
        while (t <= 1.0) {
            t += Time.deltaTime / seconds;
            enemySpriteRenderer.transform.localScale = Vector3.Lerp(newSize, originalSize, t);
            yield return null;
        }

    }

    private void GetEnemyType(PowerUpType powerUp)
    {
        switch (powerUp)
        {
            case PowerUpType.BlueDoubleJump:
                enemySpriteRenderer.material.color = Color.cyan;
                break;
            case PowerUpType.YellowZipDash:
                enemySpriteRenderer.material.color = new Color(1f, .35f, 0f, 1);
                break;
            // case PowerUpType.MagentaHoverGlide:
            //     enemySpriteRenderer.material.color = Color.magenta;
            //     break;
            case PowerUpType.Normal:
                enemySpriteRenderer.material.color = Color.white;
                break;
            default:
                break;
        }
        Debug.Log(gameObject + " is " + powerUp);
    }
    
}
