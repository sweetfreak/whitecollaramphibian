using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    BlueDoubleJump,
    RedZipDash,
    YellowHoverGlide,
    Normal
}
public class EnemyMovement : MonoBehaviour
{
    private Rigidbody2D enemyRigidbody;
    
    [SerializeField] private Animator enemyAnimator;
    public float thawTime;
    private SpriteRenderer enemySpriteRenderer;
    private float enemyFreezeSpeed = 0;
    public bool enemyIsFrozen = false;
    
    
    
    //might have the baddies be wide, so easier to jump on them
    //[SerializeField] private float enemyScale = 1f;
    
    [SerializeField] float enemyMoveSpeed = 1f;
    public  EnemyType enemyType = new EnemyType();

    void Start()
    {
        enemyRigidbody = GetComponent<Rigidbody2D>();
        enemyAnimator = GetComponent<Animator>();
        enemySpriteRenderer = GetComponent<SpriteRenderer>();
        GetEnemyType();

    }

    void Update()
    {
        enemyRigidbody.velocity = new Vector2(enemyMoveSpeed, 0);
    }
    

     void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag != "Player" )
        {
            enemyMoveSpeed = -enemyMoveSpeed;
            //Debug.Log("changing directions because bumped into " + other.tag);
            FlipEnemyFacing();
        }
    }

     public void Freeze()
     {
         enemyIsFrozen = true;
         StartCoroutine(JumpSqueeze(1.2f, .5f, 0.1f));
         StartCoroutine(ThawTimeCoroutine());
     }

     IEnumerator ThawTimeCoroutine()
     {
         enemyAnimator.enabled = false;
         enemyRigidbody.constraints = RigidbodyConstraints2D.FreezeAll;
         enemySpriteRenderer.material.color = Color.grey;
         yield return new WaitForSecondsRealtime(thawTime);
         enemyAnimator.enabled = true;
         enemyIsFrozen = false;
         GetEnemyType();
         enemyRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
     }
     
    void FlipEnemyFacing()
    {
        // Debug.Log("Flipped");
        transform.localScale = new Vector2((-1 * Mathf.Sign(-enemyRigidbody.velocity.x)), 1f);
    }
    
    IEnumerator JumpSqueeze(float xSqueeze, float ySqueeze, float seconds) {
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

        enemyMoveSpeed = -enemyMoveSpeed;
    }

    public void GetEnemyType()
    {
        switch (enemyType)
        {
            case EnemyType.BlueDoubleJump:
                enemySpriteRenderer.material.color = Color.cyan;
                break;
            case EnemyType.RedZipDash:
                enemySpriteRenderer.material.color = Color.red;
                break;
            case EnemyType.YellowHoverGlide:
                enemySpriteRenderer.material.color = Color.yellow;
                break;
            case EnemyType.Normal:
                enemySpriteRenderer.material.color = Color.white;
                break;
            default:
                break;
        }
    }
    
}
