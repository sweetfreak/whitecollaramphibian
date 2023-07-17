using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterMovement : MonoBehaviour
{
    [SerializeField] private AudioClip enterWaterSFX;
    [SerializeField] private AudioClip exitWaterSFX;
    [SerializeField] private float waterVolume = .8f;
    private void Start()
    {
       // sFXVolume = GetComponent<PlayerManager>().SFXVolume;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        { 
            //Debug.Log("Player Enter Splash Sfx");
            AudioSource.PlayClipAtPoint(enterWaterSFX, Camera.main.transform.position, waterVolume);
            
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            //Debug.Log("Player Exit Splash Sfx");
            AudioSource.PlayClipAtPoint(exitWaterSFX, Camera.main.transform.position, waterVolume);
        }
    }
}
