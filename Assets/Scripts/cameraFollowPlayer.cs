using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class cameraFollowPlayer : MonoBehaviour
{

    [SerializeField] private CinemachineVirtualCamera thisCamera;

    //[SerializeField] private bool playerExists = false;
    [SerializeField] private bool playerAlive = false;
    [SerializeField] private GameObject playerGO;
    private PlayerManager playerManager;
    private CinemachineConfiner2D confiner;
    
    void Start()
    {
        thisCamera = GetComponent<CinemachineVirtualCamera>();
        confiner = GetComponent<CinemachineConfiner2D>();
        
        FindPlayer();
        

    }

    // Update is called once per frame
    void Update()
    {
        if (!playerAlive)
        {
            FindPlayer();
            playerAlive = playerManager.playerIsAlive;
            //Debug.Log("Player alive: " + playerAlive);
        }

        if (!confiner.m_BoundingShape2D)
        {
            GameObject tilemap = GameObject.Find("BackgroundTilemap");
            confiner.m_BoundingShape2D = tilemap.GetComponent<PolygonCollider2D>();
        }
       

        if (!thisCamera.m_Follow  && playerAlive)
        {
            confiner = FindObjectOfType<CinemachineConfiner2D>();
            confiner.InvalidateCache();
            //Debug.Log("finding camera: " + thisCamera);
            thisCamera.m_Follow = playerGO.transform;
        }
        else
        {
            playerAlive = false;
        }
        
    }

    void FindPlayer()
    {
        playerManager = FindObjectOfType<PlayerManager>();
        if (!playerManager)
        {
            //Debug.Log("no player manager found");
            return;
        }
        else
        { 
            playerGO = playerManager.gameObject;

        }
    }
}
