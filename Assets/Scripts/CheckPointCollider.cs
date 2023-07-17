using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CheckPointCollider : MonoBehaviour
{
    private CheckPointManager checkPointManager;
    [SerializeField] bool checkpointNotReached = true;
    [SerializeField] private AudioClip checkpointSfx;
    [SerializeField] private float checkpointSfxVol = 1f;
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
            AudioSource.PlayClipAtPoint(checkpointSfx, Camera.main.transform.position,checkpointSfxVol);

            checkpointNotReached = false;
            checkPointManager.CheckpointReached();
        }

    }
}
