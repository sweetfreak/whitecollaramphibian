using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FruitPickup : MonoBehaviour
{
    // private AudioSource audiosource;
    [SerializeField] private AudioClip gulpSound;
    [SerializeField] private float gulpVolume = .5f;
    //[SerializeField] private int fruitsEaten;

    private bool wasCollected = false;

    // private void Start()
    // {
    //     audiosource = GetComponent<AudioSource>();
    // }

    private void OnTriggerEnter2D(Collider2D col)
    {

        if (col.tag == "Player" && !wasCollected  && (Physics2D.Raycast(transform.position + new Vector3(0, -.25f), transform.up, .5f) || Physics2D.Raycast(transform.position + new Vector3(-.25f, 0), transform.forward, .5f)))
        {
            wasCollected = true;
            FindObjectOfType<GameSession>().AddFruits();
            //Sound Effect from pixabay.com/
            // audiosource.PlayOneShot(gulpSound, gulpVolume); 
            AudioSource.PlayClipAtPoint(gulpSound, Camera.main.transform.position, gulpVolume);
            Destroy(gameObject);
            
        }
    }
}
