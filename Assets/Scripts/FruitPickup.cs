using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitPickup : MonoBehaviour
{
    [SerializeField] private AudioClip gulpSound;
    [SerializeField] private float gulpVolume = .5f;
    //[SerializeField] private int fruitsEaten;

    private bool wasCollected = false;
    
    private void OnTriggerEnter2D(Collider2D col)
    {

        if (col.tag == "Player" && !wasCollected)
        {
            wasCollected = true;
            FindObjectOfType<GameSession>().AddFruits();
            //Sound Effect from pixabay.com/
            AudioSource.PlayClipAtPoint(gulpSound, Camera.main.transform.position, gulpVolume);
            Destroy(gameObject);
            
        }
    }
}
