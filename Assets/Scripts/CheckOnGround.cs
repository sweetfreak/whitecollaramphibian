using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckOnGround : MonoBehaviour
{
    public bool isOnGround;
    public bool isWet;
    
    [Header("Collision")]
    [SerializeField] private LayerMask collisionLayer;
    [SerializeField] private LayerMask waterLayer;

    [SerializeField] float groundLengthGizmo = .3f;
    [SerializeField] private float waterLengthGizmo = .2f;
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
        
        //water stuff
        isWet = Physics2D.Raycast(transform.position, Vector2.down, waterLengthGizmo, waterLayer);
        if (isWet)
        {
            //Debug.Log("i am wet!!");
        }

    }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position + colliderOffset, transform.position + colliderOffset + Vector3.down * groundLengthGizmo);
        Gizmos.DrawLine(transform.position - colliderOffset, transform.position - colliderOffset + Vector3.down * groundLengthGizmo);
        
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * waterLengthGizmo);
        
    }
    
}
