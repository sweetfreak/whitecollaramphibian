using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CheckPointManager : MonoBehaviour
{

    //Checkpoint Reached
    //Beginning of Level Transform
    //Checkpoint Transform
    //Spawn point

    public bool spawnAtCheckpoint = false;
    [SerializeField] public GameObject startPoint;
    [SerializeField] public GameObject checkPoint;
    public Vector3 currentSpawnPoint = new Vector3();
    // public Vector3 startSpawnPoint = new Vector3();
    // public Vector3 checkPointSpawnPoint = new Vector3();

    //private Collider2D checkpointCollider;
    [SerializeField] private Animator checkPointAnimator;
    private PlayerManager playerManager;

    
    
    void Awake()
    {
        if (!spawnAtCheckpoint)
        {         
            currentSpawnPoint = startPoint.transform.position;
        }
        else
        {
            currentSpawnPoint = checkPoint.transform.position;
            checkPointAnimator.SetBool("checkpointRespawn", true);
        }
    }

    // private void Start()
    // {
    //     playerManager = FindObjectOfType<PlayerManager>();
    //     playerManager.checkpoint
    //
    // }
    

    public void CheckpointReached()
    {
        if (!spawnAtCheckpoint)
        {
            spawnAtCheckpoint = true;
            currentSpawnPoint = checkPoint.transform.position;
            checkPointAnimator.SetBool("checkpointReached", true); 
            
            // playerManager.UpdateSpawnPoint();
        }
    }
}
