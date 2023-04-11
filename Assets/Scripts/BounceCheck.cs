using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class BounceCheck : MonoBehaviour
{
    [SerializeField] private BoxCollider2D enemyBounceCollider;
    private EnemyMovement enemy;
    [SerializeField] private PhysicsMaterial2D bouncy;
    [SerializeField] private PhysicsMaterial2D frozen;
    public bool bounceOn = true;
    
   
 

    
    // Start is called before the first frame update
    void Start()
    {
        enemy = GetComponentInParent<EnemyMovement>();
        enemyBounceCollider = GetComponent<BoxCollider2D>();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Player"))
        {
            if (bounceOn)
            {
                enemy.Freeze(); 
                StartCoroutine(TurnOffBounce());
            }
            
        }
        
    }

    IEnumerator TurnOffBounce()
    {
        bounceOn = false;
        enemyBounceCollider.sharedMaterial = frozen;
        yield return new WaitForSecondsRealtime(enemy.thawTime);
        bounceOn = true;
        enemyBounceCollider.sharedMaterial = bouncy;
    }
}
