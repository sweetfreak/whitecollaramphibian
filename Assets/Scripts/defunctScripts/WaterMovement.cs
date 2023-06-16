using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterMovement : MonoBehaviour
{
    [SerializeField] private GameObject player;
    private float regularGravity = 8f;
    [SerializeField] private float waterGravity = 3f;
    //private bool isInWater = false;
    // Start is called before the first frame update
    void Start()
    {
        
        GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // CheckIfWet();
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag != "Player") {return;}
        
        Debug.Log("enter trigger");
        //isInWater = true;
        col.GetComponent<Rigidbody2D>().gravityScale = waterGravity;

    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag != "Player") {return;}
        Debug.Log("exit trigger");
        //isInWater = false;
        other.GetComponent<Rigidbody2D>().gravityScale = regularGravity;

    }

    // void CheckIfWet()
    // {
    //     if (isInWater)
    //     {
    //         Debug.Log("is wet now");
    //         this.player.GetComponent<Rigidbody2D>().gravityScale = waterGravity;
    //     }
    //     else
    //     {
    //         this.player.GetComponent<Rigidbody2D>().gravityScale = regularGravity;
    //
    //     }
    // }
}
