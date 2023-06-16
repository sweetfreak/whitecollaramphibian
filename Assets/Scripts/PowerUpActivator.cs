using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public enum PowerUpType
{
    BlueDoubleJump,
    RedZipDash,
    YellowHoverGlide,
    Normal
}

public class PowerUpActivator : MonoBehaviour
{
    [SerializeField] private BoxCollider2D enemyBounceCollider;
    private EnemyMovement activator;
    [SerializeField] private PhysicsMaterial2D bouncy;
    [SerializeField] private PhysicsMaterial2D frozen;
    public bool bounceOn = true;
    public PowerUpType powerUpType;

    public float bounceForce = 1f;
    //public bool powerUpIsActivated = true;

    

    void Start()
    {
        activator = GetComponentInParent<EnemyMovement>();
        enemyBounceCollider = GetComponent<BoxCollider2D>();
        bounceOn = true;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        //if the player collides while bounce is on and the r velocity is very low, it'll still bounce the player upward
        //NOTE: I don't think this works?
        if (col.gameObject.CompareTag("Player") && bounceOn && Mathf.Abs(col.rigidbody.velocity.y) < 1f )
        {
            col.rigidbody.velocity = new Vector2(0,   col.rigidbody.velocity.y * bounceForce);
        }
    }

    public void TempDeactivate()
    {
        //Debug.Log("temp Deactivate method started");
        if (bounceOn)
        {
            activator.Freeze();
            StartCoroutine(TurnOffBounce());
        }
    }

    IEnumerator TurnOffBounce()
    {
        bounceOn = false;
        enemyBounceCollider.sharedMaterial = frozen;
        yield return new WaitForSecondsRealtime(activator.thawTime);
        bounceOn = true;
        enemyBounceCollider.sharedMaterial = bouncy;
    }
}
