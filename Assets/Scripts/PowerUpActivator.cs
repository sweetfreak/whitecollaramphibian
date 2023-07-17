using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum PowerUpType
{
    BlueDoubleJump,
    YellowZipDash,
    //MagentaHoverGlide,
    Normal
}

public class PowerUpActivator : MonoBehaviour
{
    //[SerializeField] private BoxCollider2D enemyBounceCollider;
    private EnemyMovement activator;
    //[SerializeField] private PhysicsMaterial2D bouncy;
    //[SerializeField] private PhysicsMaterial2D frozen;
   
    public bool bounceOn = true;
    public PowerUpType powerUpType;

    public float bounceForce = 1f;
    //public bool powerUpIsActivated = true;

    [SerializeField] private bool isEnemy = true; 
    

    void Start()
    {
        if (isEnemy)
        {
            activator = GetComponentInParent<EnemyMovement>();
            //enemyBounceCollider = GetComponent<BoxCollider2D>();
        }
        
        bounceOn = true;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        //if player bounces on enemy
        if (col.gameObject.CompareTag("Player") && bounceOn) 
        {
           //add vertical force upward
            col.rigidbody.velocity = new Vector2(col.rigidbody.velocity.x,    bounceForce);
            //and if enemy, deactivate them
            if (isEnemy)
            { 
                TempDeactivate();
            }
        }
    }

    public void TempDeactivate()
    {
        //Debug.Log("temp Deactivate method started");
        if (bounceOn)
        {
            //freeze the enemy attached to this
            activator.Freeze();
            StartCoroutine(TurnOffBounce());
        }
    }

    IEnumerator TurnOffBounce()
    {
        bounceOn = false;
        //enemyBounceCollider.sharedMaterial = frozen;
        yield return new WaitForSecondsRealtime(activator.thawTime);
        bounceOn = true;
        //enemyBounceCollider.sharedMaterial = bouncy;
    }
}
