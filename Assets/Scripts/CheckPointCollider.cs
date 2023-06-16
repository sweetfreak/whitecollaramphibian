using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CheckPointCollider : MonoBehaviour
{
    private CheckPointManager checkPointManager;
    [SerializeField] bool checkpointNotReached = true;
    private void Start()
    {
        checkPointManager = FindObjectOfType<CheckPointManager>();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //Debug.Log("collision detected with Checkpoint");
        if (col.gameObject.CompareTag("Player") && checkpointNotReached)
        {
            //Debug.Log("PLAYER reached checkpoint");
            checkpointNotReached = false;
            checkPointManager.CheckpointReached();
        }

    }
}
