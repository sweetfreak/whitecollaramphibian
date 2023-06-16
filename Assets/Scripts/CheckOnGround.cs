using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckOnGround : MonoBehaviour
{
    public bool isOnGround;
    
    [Header("Collision")]
    [SerializeField] private LayerMask collisionLayer;
  
    [SerializeField] float groundLengthGizmo = .3f;
    [SerializeField] private Vector3 colliderOffset;
    private Animator myAnimator;

    private void Start()
    {
        myAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        isOnGround = 
            Physics2D.Raycast(transform.position + colliderOffset, Vector2.down, groundLengthGizmo, collisionLayer ) 
            || Physics2D.Raycast(transform.position - colliderOffset, Vector2.down, groundLengthGizmo, collisionLayer);

        if (isOnGround)
        {
            myAnimator.SetBool("isOnGround", true);
        }
        else
        {
            myAnimator.SetBool("isOnGround", false);

        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + colliderOffset, transform.position + colliderOffset + Vector3.down * groundLengthGizmo);
        Gizmos.DrawLine(transform.position - colliderOffset, transform.position - colliderOffset + Vector3.down * groundLengthGizmo);
    }
    
}
